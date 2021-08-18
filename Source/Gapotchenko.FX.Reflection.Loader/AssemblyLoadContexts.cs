#if TFF_ASSEMBLYLOADCONTEXT

using System.Runtime.Loader;

namespace Gapotchenko.FX.Reflection.Loader
{
    static class AssemblyLoadContexts
    {
        public static AssemblyLoadContext Local { get; } =
            AssemblyLoadContext.GetLoadContext(typeof(AssemblyLoadContexts).Assembly) ??
            AssemblyLoadContext.Default;

        public static AssemblyLoadContext Current =>
#if NETCOREAPP3_0_OR_GREATER
            AssemblyLoadContext.CurrentContextualReflectionContext ??
#endif
            Local;
    }
}

#endif
