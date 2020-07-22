using Gapotchenko.FX.Reflection.Loader.Pal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Backends
{
    class ProbingPathAssemblyLoaderBackend : IAssemblyLoaderBackend
    {
        public ProbingPathAssemblyLoaderBackend(params string[] probingPaths)
        {
            _ProbingPaths = probingPaths;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        public bool StrictVersionMatch { get; set; }

        string[]? _ProbingPaths;

        static IEnumerable<string> _EnumerateAssemblies(string path)
        {
            foreach (var i in Directory.EnumerateFiles(path, "*.dll"))
                yield return i;
            foreach (var i in Directory.EnumerateFiles(path, "*.exe"))
                yield return i;
        }

        List<KeyValuePair<string, AssemblyName>>? _CachedProbingList;

        List<KeyValuePair<string, AssemblyName>> _GetProbingList()
        {
            if (_CachedProbingList == null)
                lock (this)
                    if (_CachedProbingList == null)
                    {
                        _CachedProbingList = new List<KeyValuePair<string, AssemblyName>>();

                        if (_ProbingPaths != null)
                        {
                            foreach (string path in _ProbingPaths)
                            {
                                if (path == null)
                                    continue;

                                if (!Directory.Exists(path))
                                    continue;

                                foreach (string file in _EnumerateAssemblies(path))
                                {
                                    AssemblyName definition;
                                    try
                                    {
                                        definition = AssemblyName.GetAssemblyName(file);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                    _CachedProbingList.Add(new KeyValuePair<string, AssemblyName>(file, definition));
                                }
                            }

                            // Clear probing paths in order to free the memory.
                            _ProbingPaths = null;
                        }
                    }

            return _CachedProbingList;
        }

        readonly ConcurrentDictionary<string, Assembly?> _ResolvedAssembliesCache = new ConcurrentDictionary<string, Assembly?>(StringComparer.OrdinalIgnoreCase);

        bool _ReferenceMatchesDefinition(AssemblyName reference, AssemblyName definition) =>
            StringComparer.OrdinalIgnoreCase.Equals(reference.Name, definition.Name) &&
            _ReferenceMatchesDefinition(reference.Version, definition.Version) &&
            ArrayEqualityComparer.Equals(reference.GetPublicKeyToken(), definition.GetPublicKeyToken()) &&
#if NET40
            (reference.CultureInfo is null && definition.CultureInfo is null || (reference.CultureInfo?.Equals(definition.CultureInfo) ?? false))
#else
            string.Equals(reference.CultureName, definition.CultureName, StringComparison.OrdinalIgnoreCase)
#endif
            ;

        bool _ReferenceMatchesDefinition(Version? reference, Version? definition)
        {
            if (StrictVersionMatch)
            {
                return reference == definition;
            }
            else
            {
                if (reference == definition)
                    return true;
                if (reference is null || definition is null)
                    return false;

                return reference <= definition;
            }
        }

        Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            var name = args.Name;
            if (name == null)
                return null;

            if (IsAssemblyResolutionInhibited(args))
                return null;

            if (_ResolvedAssembliesCache.TryGetValue(name, out var assembly))
                return assembly;

            var reference = new AssemblyName(name);

            foreach (var i in _GetProbingList())
            {
                AssemblyName definition = i.Value;
                if (_ReferenceMatchesDefinition(reference, definition))
                {
                    assembly = LoadAssembly(i.Key, definition);
                    break;
                }
            }

            _ResolvedAssembliesCache.TryAdd(name, assembly);
            return assembly;
        }

        protected virtual bool IsAssemblyResolutionInhibited(ResolveEventArgs args) => false;

        protected virtual Assembly LoadAssembly(string filePath, AssemblyName name) => Assembly.LoadFrom(filePath);
    }
}
