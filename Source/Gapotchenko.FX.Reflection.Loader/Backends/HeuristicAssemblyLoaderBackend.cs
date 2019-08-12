using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        protected override bool IsAssemblyResolutionInhibited(ResolveEventArgs args) =>
            _AssemblyDependencyTracker?.IsAssemblyResolutionInhibited(args.RequestingAssembly) ?? true;

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
