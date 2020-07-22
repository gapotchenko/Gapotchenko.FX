using Gapotchenko.FX.Reflection.Loader.Backends;
using Gapotchenko.FX.Reflection.Loader.Pal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader
{
    sealed class AssemblyDescriptor : IDisposable
    {
        public AssemblyDescriptor(Assembly assembly, string[]? additionalProbingPaths)
        {
            _AssemblyDependencyTracker = new AssemblyDependencyTracker(assembly);

            string assemblyFilePath = new Uri(assembly.EscapedCodeBase).LocalPath;

            if (BindingRedirectAssemblyLoaderBackend.TryCreate(assemblyFilePath, _AssemblyDependencyTracker, out _AssemblyLoaderBackend))
            {
                _HasBindingRedirects = _AssemblyLoaderBackend != null;
                AddProbingPaths(additionalProbingPaths);
            }
            else
            {
                var probingPaths = new List<string>();

                var path = Path.GetDirectoryName(assemblyFilePath);
                if (!string.IsNullOrEmpty(path))
                    probingPaths.Add(path);

                if (additionalProbingPaths != null)
                    _AccumulateNewProbingPaths(probingPaths, additionalProbingPaths);

                _AssemblyLoaderBackend = new HeuristicAssemblyLoaderBackend(_AssemblyDependencyTracker, probingPaths.ToArray());
            }
        }

        readonly AssemblyDependencyTracker _AssemblyDependencyTracker;
        readonly IAssemblyLoaderBackend? _AssemblyLoaderBackend;
        readonly bool _HasBindingRedirects;

        HashSet<string>? _ProbingPaths;
        List<HeuristicAssemblyLoaderBackend>? _ProbingPathAssemblyLoaderBackends;

        void _AccumulateNewProbingPaths(List<string> accumulator, string[] probingPaths)
        {
            foreach (var i in probingPaths)
            {
                if (i == null)
                    continue;

                string probingPath = Path.GetFullPath(i);

                if (_ProbingPaths == null)
                    _ProbingPaths = new HashSet<string>(FileSystem.PathComparer);

                if (_ProbingPaths.Add(probingPath))
                    accumulator.Add(probingPath);
            }
        }

        public bool AddProbingPaths(string[]? probingPaths)
        {
            if (probingPaths == null)
                return false;

            var newProbingPaths = new List<string>();
            _AccumulateNewProbingPaths(newProbingPaths, probingPaths);

            if (newProbingPaths.Count == 0)
                return false;

            if (_ProbingPathAssemblyLoaderBackends == null)
                _ProbingPathAssemblyLoaderBackends = new List<HeuristicAssemblyLoaderBackend>();

            _ProbingPathAssemblyLoaderBackends.Add(
                new HeuristicAssemblyLoaderBackend(_AssemblyDependencyTracker, newProbingPaths.ToArray())
                {
                    StrictVersionMatch = _HasBindingRedirects
                });

            return true;
        }

        public void Dispose()
        {
            _AssemblyLoaderBackend?.Dispose();

            if (_ProbingPathAssemblyLoaderBackends != null)
                foreach (var backend in _ProbingPathAssemblyLoaderBackends)
                    backend.Dispose();
        }
    }
}
