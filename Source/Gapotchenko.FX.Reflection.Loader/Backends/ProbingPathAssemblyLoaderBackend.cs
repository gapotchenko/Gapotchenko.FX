using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Backends
{
    class ProbingPathAssemblyLoaderBackend : IAssemblyLoaderBackend
    {
        public ProbingPathAssemblyLoaderBackend(AssemblyLoadPal assemblyLoadPal, params string[] probingPaths)
        {
            AssemblyLoadPal = assemblyLoadPal;
            m_ProbingPaths = probingPaths;

            AssemblyLoadPal.Resolving += AssemblyResolver_Resolving;
        }

        public void Dispose()
        {
            AssemblyLoadPal.Resolving -= AssemblyResolver_Resolving;
        }

        public bool StrictVersionMatch { get; set; }

        protected readonly AssemblyLoadPal AssemblyLoadPal;
        readonly string[] m_ProbingPaths;

        static IEnumerable<string> _EnumerateAssemblies(string path) =>
            Directory.EnumerateFiles(path, "*.dll")
            .Concat(Directory.EnumerateFiles(path, "*.exe"));

        List<KeyValuePair<string, AssemblyName>>? _CachedProbingList;

        List<KeyValuePair<string, AssemblyName>> _GetProbingList()
        {
            if (_CachedProbingList == null)
                lock (this)
                    if (_CachedProbingList == null)
                    {
                        _CachedProbingList = new();

                        foreach (string path in m_ProbingPaths)
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

        Assembly? AssemblyResolver_Resolving(AssemblyLoadPal sender, AssemblyLoadPal.ResolvingEventArgs args)
        {
            if (IsAssemblyResolutionInhibited(args.RequestingAssembly))
                return null;

            var name = args.FullName;

            if (_ResolvedAssembliesCache.TryGetValue(name, out var assembly))
                return assembly;

            var reference = args.Name;

            foreach (var i in _GetProbingList())
            {
                var definition = i.Value;
                if (_ReferenceMatchesDefinition(reference, definition))
                {
                    assembly = LoadAssembly(i.Key, definition);
                    break;
                }
            }

            _ResolvedAssembliesCache.TryAdd(name, assembly);
            return assembly;
        }

        protected virtual bool IsAssemblyResolutionInhibited(Assembly? requestingAssembly) => false;

        protected virtual Assembly LoadAssembly(string filePath, AssemblyName name) => AssemblyLoadPal.LoadFrom(filePath);

        public string? ResolveAssemblyPath(AssemblyName assemblyName)
        {
            if (_ResolvedAssembliesCache.TryGetValue(assemblyName.FullName, out var assembly))
                return assembly?.Location;

            var reference = assemblyName;

            foreach (var i in _GetProbingList())
            {
                var definition = i.Value;
                if (_ReferenceMatchesDefinition(reference, definition))
                    return i.Key;
            }

            return null;
        }

        public string? ResolveUnmanagedDllPath(string unmanagedDllName)
        {
            bool isRelativePath =
#if NETCOREAPP2_1_OR_GREATER
                !Path.IsPathFullyQualified(unmanagedDllName);
#else
                !Path.IsPathRooted(unmanagedDllName);
#endif

            foreach (LibraryNameVariation libraryNameVariation in LibraryNameVariation.Enumerate(unmanagedDllName, isRelativePath))
            {
                string libraryName = libraryNameVariation.Prefix + unmanagedDllName + libraryNameVariation.Suffix;
                foreach (string probingPath in m_ProbingPaths)
                {
                    string libraryPath = Path.Combine(probingPath, libraryName);
                    if (File.Exists(libraryPath))
                        return libraryPath;
                }
            }

            return null;
        }
    }
}
