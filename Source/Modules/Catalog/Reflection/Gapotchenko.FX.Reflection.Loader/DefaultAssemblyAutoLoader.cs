// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Runtime.CompilerServices;
#if TFF_ASSEMBLYLOADCONTEXT
using System.Runtime.Loader;
#endif

namespace Gapotchenko.FX.Reflection.Loader;

sealed class DefaultAssemblyAutoLoader : AssemblyAutoLoader
{
#if TFF_ASSEMBLYLOADCONTEXT
    DefaultAssemblyAutoLoader(AssemblyLoadContext assemblyLoadContext) :
        base(assemblyLoadContext)
    {
    }
#else
    DefaultAssemblyAutoLoader()
    {
    }
#endif

#if TFF_ASSEMBLYLOADCONTEXT

#if NETCOREAPP3_0_OR_GREATER
    public static DefaultAssemblyAutoLoader Instance
    {
        get
        {
            // The default instance of assembly auto loader depends on the current assembly load context.
            var assemblyLoadContext = AssemblyLoadContexts.Current;

            if (!m_Instances.TryGetValue(assemblyLoadContext, out var instance))
            {
                // AssemblyLoadContexts.Current is AsyncLocal and thus can have the same value in several threads simultaneously.
                // This may lead to a race condition which should be avoided by providing a strong 1-to-1 mapping between
                // AssemblyLoadContext <-> AssemblyAutoLoader instances.
                lock (m_Instances)
                {
                    if (!m_Instances.TryGetValue(assemblyLoadContext, out instance))
                    {
                        instance = new DefaultAssemblyAutoLoader(assemblyLoadContext);
                        m_Instances.Add(assemblyLoadContext, instance);
                    }
                }
            }

            return instance;
        }
    }

    static readonly ConditionalWeakTable<AssemblyLoadContext, DefaultAssemblyAutoLoader> m_Instances = [];
#else
    public static DefaultAssemblyAutoLoader Instance { get; } = new(AssemblyLoadContexts.Local);
#endif

#else

    public static DefaultAssemblyAutoLoader Instance { get; } = new();

#endif

#pragma warning disable CA2215 // Dispose methods should call base class dispose
    protected override void Dispose(bool disposing)
#pragma warning restore CA2215 // Dispose methods should call base class dispose
    {
        // Default instance is not disposable by design.
    }
}
