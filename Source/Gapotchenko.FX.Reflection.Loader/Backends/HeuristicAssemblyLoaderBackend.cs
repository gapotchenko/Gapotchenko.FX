using Gapotchenko.FX.Reflection.Pal;
using System;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Backends
{
    sealed class HeuristicAssemblyLoaderBackend : ProbingPathAssemblyLoaderBackend
    {
        public HeuristicAssemblyLoaderBackend(AssemblyDependencyTracker assemblyDependencyTracker, params string[] probingPaths) :
            base(probingPaths)
        {
            _AssemblyDependencyTracker = assemblyDependencyTracker;
        }

        readonly AssemblyDependencyTracker _AssemblyDependencyTracker;

        protected override bool IsAssemblyResolutionInhibited(AssemblyResolver.ResolvingEventArgs args) =>
            _AssemblyDependencyTracker.IsAssemblyResolutionInhibited(args.RequestingAssembly);

        protected override Assembly LoadAssembly(string filePath, AssemblyName name)
        {
            bool registered = _AssemblyDependencyTracker.RegisterReferencedAssembly(name);
            try
            {
                return base.LoadAssembly(filePath, name);
            }
            catch
            {
                if (registered)
                    _AssemblyDependencyTracker.UnregisterReferencedAssembly(name);
                throw;
            }
        }
    }
}
