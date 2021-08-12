using System;
using System.Diagnostics;
using System.Reflection;
#if TFF_ASSEMBLYLOADCONTEXT
using System.Runtime.Loader;
#endif

namespace Gapotchenko.FX.Reflection.Pal
{
    /// <summary>
    /// Platform abstraction for assembly resolver API.
    /// </summary>
    sealed class AssemblyResolver
    {
#if TFF_ASSEMBLYLOADCONTEXT
        public AssemblyResolver(AppDomain appDomain, AssemblyLoadContext assemblyLoadContext)
        {
            m_AppDomain = appDomain;
            m_AssemblyLoadContext = assemblyLoadContext;
        }
#else
        public AssemblyResolver(AppDomain appDomain)
        {
            m_AppDomain = appDomain;
        }
#endif

        public static AssemblyResolver Default { get; } =
#if TFF_ASSEMBLYLOADCONTEXT
            new AssemblyResolver(AppDomain.CurrentDomain, AssemblyLoadContext.Default);
#else
            new AssemblyResolver(AppDomain.CurrentDomain);
#endif

#if TFF_ASSEMBLYLOADCONTEXT
        AssemblyLoadContext m_AssemblyLoadContext;
#endif
        AppDomain m_AppDomain;

        public sealed class ResolvingEventArgs : EventArgs
        {
            public ResolvingEventArgs(Assembly? requestingAssembly, string name)
            {
                RequestingAssembly = requestingAssembly;
                m_FullName = name;
            }

#if TFF_ASSEMBLYLOADCONTEXT
            public ResolvingEventArgs(AssemblyName name)
            {
                m_Name = name;
            }
#endif

            public Assembly? RequestingAssembly { get; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if TFF_ASSEMBLYLOADCONTEXT
            string? m_FullName;
#else
            string m_FullName;
#endif

            public string FullName =>
#if TFF_ASSEMBLYLOADCONTEXT
                m_FullName ??= (m_Name ?? throw new InvalidOperationException()).FullName;
#else
                m_FullName;
#endif

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            AssemblyName? m_Name;

            public AssemblyName Name => m_Name ??= new AssemblyName(m_FullName ?? throw new InvalidOperationException());
        }

        public delegate Assembly? ResolvingEventHandler(AssemblyResolver sender, ResolvingEventArgs args);

        ResolvingEventHandler? m_Resolving;

        public event ResolvingEventHandler Resolving
        {
            add
            {
                lock (this)
                {
                    bool was = m_Resolving != null;
                    m_Resolving += value;
                    if (!was && m_Resolving != null)
                        SetupResolving();
                }
            }
            remove
            {
                lock (this)
                {
                    bool was = m_Resolving != null;
                    m_Resolving -= value;
                    if (was && m_Resolving == null)
                        TeardownResolving();
                }
            }
        }

        void SetupResolving()
        {
            m_AppDomain.AssemblyResolve += AppDomain_AssemblyResolve;
#if TFF_ASSEMBLYLOADCONTEXT
            m_AssemblyLoadContext.Resolving += AssemblyLoadContext_Resolving;
#endif
        }

        void TeardownResolving()
        {
#if TFF_ASSEMBLYLOADCONTEXT
            m_AssemblyLoadContext.Resolving -= AssemblyLoadContext_Resolving;
#endif
            m_AppDomain.AssemblyResolve -= AppDomain_AssemblyResolve;
        }

        Assembly? AppDomain_AssemblyResolve(object? sender, ResolveEventArgs args) =>
            InvokeResolving(new ResolvingEventArgs(args.RequestingAssembly, args.Name));

#if TFF_ASSEMBLYLOADCONTEXT
        Assembly? AssemblyLoadContext_Resolving(AssemblyLoadContext assemblyLoadContext, AssemblyName name) =>
            InvokeResolving(new ResolvingEventArgs(name));
#endif

        Assembly? InvokeResolving(ResolvingEventArgs args)
        {
            var eh = m_Resolving;
            if (eh != null)
            {
                foreach (ResolvingEventHandler handler in eh.GetInvocationList())
                {
                    var assembly = handler(this, args);
                    if (assembly != null)
                        return assembly;
                }
            }
            return null;
        }
    }
}
