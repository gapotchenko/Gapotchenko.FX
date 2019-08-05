using Gapotchenko.FX.Reflection.Pal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Reflection
{
    sealed class AssemblyDescriptor : IDisposable
    {
        public AssemblyDescriptor(Assembly assembly, string[] additionalProbingPaths)
        {
            _Assembly = assembly;
            _AssemblyDependencyTracker = new AssemblyDependencyTracker(assembly);

            string assemblyFilePath = new Uri(assembly.EscapedCodeBase).LocalPath;

            if (BindingRedirectAssemblyLoader.TryCreate(assemblyFilePath, _AssemblyDependencyTracker, out _AssemblyLoader))
            {
                _HasBindingRedirects = _AssemblyLoader != null;
                AddProbingPaths(additionalProbingPaths);
            }
            else
            {
                var probingPaths = new List<string>();
                probingPaths.Add(Path.GetDirectoryName(assemblyFilePath));
                if (additionalProbingPaths != null)
                    _AccumulateNewProbingPaths(probingPaths, additionalProbingPaths);

                _AssemblyLoader = new HeuristicAssemblyLoader(_AssemblyDependencyTracker, probingPaths.ToArray());
            }
        }

        Assembly _Assembly;
        AssemblyDependencyTracker _AssemblyDependencyTracker;
        IAssemblyLoader _AssemblyLoader;
        bool _HasBindingRedirects;

        HashSet<string> _ProbingPaths;
        List<HeuristicAssemblyLoader> _ProbingPathAssemblyLoaders;

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

        public bool AddProbingPaths(string[] probingPaths)
        {
            if (probingPaths == null)
                return false;

            var newProbingPaths = new List<string>();
            _AccumulateNewProbingPaths(newProbingPaths, probingPaths);

            if (newProbingPaths.Count == 0)
                return false;

            if (_ProbingPathAssemblyLoaders == null)
                _ProbingPathAssemblyLoaders = new List<HeuristicAssemblyLoader>();

            _ProbingPathAssemblyLoaders.Add(
                new HeuristicAssemblyLoader(_AssemblyDependencyTracker, newProbingPaths.ToArray())
                {
                    StrictVersionMatch = _HasBindingRedirects
                });

            return true;
        }

        public void Dispose()
        {
            _AssemblyLoader?.Dispose();

            if (_ProbingPathAssemblyLoaders != null)
                foreach (var i in _ProbingPathAssemblyLoaders)
                    i.Dispose();
        }
    }
}
