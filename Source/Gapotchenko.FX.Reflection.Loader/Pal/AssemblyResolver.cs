using System;
using System.Diagnostics;
using System.Reflection;
#if TFF_ASSEMBLYLOADCONTEXT
using System.Runtime.Loader;
#endif

namespace Gapotchenko.FX.Reflection.Pal
{
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

            public ResolvingEventArgs(AssemblyName name)
            {
                m_Name = name;
            }

            public Assembly? RequestingAssembly { get; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string? m_FullName;

            public string FullName => m_FullName ??= (m_Name ?? throw new InvalidOperationException()).FullName;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            AssemblyName? m_Name;

            public AssemblyName Name => m_Name ??= new AssemblyName(m_FullName ?? throw new InvalidOperationException());
        }

        EventHandler<ResolvingEventArgs>? m_Resolving;

        public event EventHandler<ResolvingEventArgs> Resolving
        {
            add
            {
                m_Resolving += value;
            }
            remove
            {
                m_Resolving -= value;
            }
        }
    }
}
