using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Backends
{
    sealed class HeuristicAssemblyLoaderBackend : ProbingPathAssemblyLoaderBackend
    {
        public HeuristicAssemblyLoaderBackend(AssemblyLoadPal assemblyLoadPal, AssemblyDependencyTracker assemblyDependencyTracker, params string[] probingPaths) :
            base(assemblyLoadPal, probingPaths)
        {
            _AssemblyDependencyTracker = assemblyDependencyTracker;
        }

        readonly AssemblyDependencyTracker _AssemblyDependencyTracker;

        protected override bool IsAssemblyResolutionInhibited(AssemblyLoadPal.ResolvingEventArgs args) =>
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
