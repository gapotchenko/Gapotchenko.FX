using System;
using System.Diagnostics;
using System.Reflection;
#if TFF_ASSEMBLYLOADCONTEXT
using System.Runtime.Loader;
#endif

namespace Gapotchenko.FX.Reflection
{
    /// <summary>
    /// Platform abstraction layer for assembly loading functionality of a host environment.
    /// </summary>
    sealed class AssemblyLoadPal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyAutoLoader"/> class for the specified app domain.
        /// </summary>
        public AssemblyLoadPal(AppDomain appDomain)
        {
            if (appDomain == null)
                throw new ArgumentNullException(nameof(appDomain));

            m_AppDomain = appDomain;
        }

#if TFF_ASSEMBLYLOADCONTEXT
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyAutoLoader"/> class for the specified assembly load context.
        /// </summary>
        public AssemblyLoadPal(AssemblyLoadContext assemblyLoadContext)
        {
            if (assemblyLoadContext == null)
                throw new ArgumentNullException(nameof(assemblyLoadContext));

            m_AssemblyLoadContext = assemblyLoadContext;
        }

        AssemblyLoadPal(AppDomain appDomain, AssemblyLoadContext assemblyLoadContext)
        {
            m_AppDomain = appDomain;
            m_AssemblyLoadContext = assemblyLoadContext;
        }
#endif

#if TFF_ASSEMBLYLOADCONTEXT
        /// <summary>
        /// Gets the default instance of <see cref="AssemblyLoadPal"/>.
        /// The instance handles the default assembly load context.
        /// </summary>
#else
        /// <summary>
        /// Gets the default instance of <see cref="AssemblyLoadPal"/>.
        /// The default instance handles the current app domain.
        /// </summary>
#endif
        public static AssemblyLoadPal Default { get; } =
#if TFF_ASSEMBLYLOADCONTEXT
            new AssemblyLoadPal(AppDomain.CurrentDomain, AssemblyLoadContext.Default);
#else
            new AssemblyLoadPal(AppDomain.CurrentDomain);
#endif

#if TFF_ASSEMBLYLOADCONTEXT
        readonly AssemblyLoadContext? m_AssemblyLoadContext;
#endif
        readonly AppDomain? m_AppDomain;

        public sealed class ResolvingEventArgs : EventArgs
        {
            public ResolvingEventArgs(Assembly? requestingAssembly, string name)
            {
                RequestingAssembly = requestingAssembly;
                m_FullName = name;
            }

#if TFF_ASSEMBLYLOADCONTEXT
            internal ResolvingEventArgs(AssemblyName name)
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

        /// <summary>
        /// Represents a method that handles the <see cref="Resolving"/> event of an <see cref="AssemblyLoadPal"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The resolved assembly or null if the assembly cannot be resolved.</returns>
        public delegate Assembly? ResolvingEventHandler(AssemblyLoadPal sender, ResolvingEventArgs args);

        ResolvingEventHandler? m_Resolving;

        /// <summary>
        /// Occurs when the resolution of an assembly fails.
        /// </summary>
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
            if (m_AppDomain != null)
                m_AppDomain.AssemblyResolve += AppDomain_AssemblyResolve;

#if TFF_ASSEMBLYLOADCONTEXT
            if (m_AppDomain == null && m_AssemblyLoadContext != null)
                m_AssemblyLoadContext.Resolving += AssemblyLoadContext_Resolving;
#endif
        }

        void TeardownResolving()
        {
#if TFF_ASSEMBLYLOADCONTEXT
            if (m_AppDomain == null && m_AssemblyLoadContext != null)
                m_AssemblyLoadContext.Resolving -= AssemblyLoadContext_Resolving;
#endif

            if (m_AppDomain != null)
                m_AppDomain.AssemblyResolve -= AppDomain_AssemblyResolve;
        }

        Assembly? AppDomain_AssemblyResolve(object? sender, ResolveEventArgs args) =>
            InvokeResolving(new ResolvingEventArgs(args.RequestingAssembly, args.Name!));

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

        public Assembly LoadFrom(string assemblyFile)
        {
#if TFF_ASSEMBLYLOADCONTEXT
            if (m_AssemblyLoadContext != null)
                return m_AssemblyLoadContext.LoadFromAssemblyPath(assemblyFile);
#endif

            if (m_AppDomain != null)
                return Assembly.LoadFrom(assemblyFile);  // TODO: change to AppDomain method

            throw new InvalidOperationException();
        }

        public Assembly Load(AssemblyName assemblyName)
        {
#if TFF_ASSEMBLYLOADCONTEXT
            if (m_AssemblyLoadContext != null)
                return m_AssemblyLoadContext.LoadFromAssemblyName(assemblyName);
#endif

            if (m_AppDomain != null)
                return Assembly.Load(assemblyName);  // TODO: change to AppDomain method

            throw new InvalidOperationException();
        }
    }
}
