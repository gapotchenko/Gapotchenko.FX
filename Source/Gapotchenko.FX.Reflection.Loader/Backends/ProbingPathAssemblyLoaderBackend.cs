using Gapotchenko.FX.Reflection.Loader.Pal;
using Gapotchenko.FX.Reflection.Pal;
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

            AssemblyResolver.Default.Resolving += AssemblyResolver_Resolving;
        }

        public void Dispose()
        {
            AssemblyResolver.Default.Resolving -= AssemblyResolver_Resolving;
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
                        _CachedProbingList = new();

                        if (_ProbingPaths != null)
                        {
                            foreach (string path in _ProbingPaths)
                            {
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
                                    _CachedProbingList.Add(new(file, definition));
                                }
                            }

                            // Clear probing paths in order to free the memory.
                            _ProbingPaths = null;
                        }
                    }

            return _CachedProbingList;
        }

        readonly ConcurrentDictionary<string, Assembly?> _ResolvedAssembliesCache = new(StringComparer.OrdinalIgnoreCase);

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

        Assembly? AssemblyResolver_Resolving(AssemblyResolver sender, AssemblyResolver.ResolvingEventArgs args)
        {
            if (IsAssemblyResolutionInhibited(args))
                return null;

            var name = args.FullName;

            if (_ResolvedAssembliesCache.TryGetValue(name, out var assembly))
                return assembly;

            var reference = args.Name;

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

        protected virtual bool IsAssemblyResolutionInhibited(AssemblyResolver.ResolvingEventArgs args) => false;

        protected virtual Assembly LoadAssembly(string filePath, AssemblyName name) => Assembly.LoadFrom(filePath);
    }
}
