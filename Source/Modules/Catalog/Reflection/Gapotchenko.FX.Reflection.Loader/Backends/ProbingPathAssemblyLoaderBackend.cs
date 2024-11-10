// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System.Collections.Concurrent;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Backends;

class ProbingPathAssemblyLoaderBackend : IAssemblyLoaderBackend
{
    public ProbingPathAssemblyLoaderBackend(bool isAttached, AssemblyLoadPal assemblyLoadPal, IReadOnlyList<string> probingPaths)
    {
        AssemblyLoadPal = assemblyLoadPal;
        m_ProbingPaths = probingPaths;

        if (isAttached)
            AssemblyLoadPal.Resolving += AssemblyResolver_Resolving;
    }

    public void Dispose()
    {
        AssemblyLoadPal.Resolving -= AssemblyResolver_Resolving;
    }

    public bool StrictVersionMatch { get; init; }

    protected readonly AssemblyLoadPal AssemblyLoadPal;
    readonly IReadOnlyList<string> m_ProbingPaths;

    static IEnumerable<string> EnumerateAssemblies(string directoryPath) =>
        Directory.EnumerateFiles(directoryPath, "*.dll")
        .Concat(Directory.EnumerateFiles(directoryPath, "*.exe"));

    List<KeyValuePair<string, AssemblyName>>? m_CachedProbingList;

    List<KeyValuePair<string, AssemblyName>> GetProbingList()
    {
        if (m_CachedProbingList == null)
            lock (this)
                m_CachedProbingList ??= GetProbingListCore(m_ProbingPaths);

        return m_CachedProbingList;
    }

    static List<KeyValuePair<string, AssemblyName>> GetProbingListCore(IEnumerable<string> probingPaths)
    {
        var list = new List<KeyValuePair<string, AssemblyName>>();

        foreach (string path in probingPaths)
        {
            if (!Directory.Exists(path))
                continue;

            foreach (string file in EnumerateAssemblies(path))
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

                list.Add(new(file, definition));
            }
        }

        return list;
    }

    readonly ConcurrentDictionary<string, Assembly?> m_ResolvedAssembliesCache = new(StringComparer.OrdinalIgnoreCase);

    bool ReferenceMatchesDefinition(AssemblyName reference, AssemblyName definition) =>
        StringComparer.OrdinalIgnoreCase.Equals(reference.Name, definition.Name) &&
        ReferenceMatchesDefinition(reference.Version, definition.Version) &&
        ArrayEqualityComparer.Equals(reference.GetPublicKeyToken(), definition.GetPublicKeyToken()) &&
        (reference.CultureInfo is null && definition.CultureInfo is null || (reference.CultureInfo?.Equals(definition.CultureInfo) ?? false));

    bool ReferenceMatchesDefinition(Version? reference, Version? definition)
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

        if (m_ResolvedAssembliesCache.TryGetValue(name, out var assembly))
            return assembly;

        var reference = args.Name;

        foreach (var i in GetProbingList())
        {
            var definition = i.Value;
            if (ReferenceMatchesDefinition(reference, definition))
            {
                assembly = LoadAssembly(i.Key, definition);
                break;
            }
        }

        m_ResolvedAssembliesCache.TryAdd(name, assembly);
        return assembly;
    }

    protected virtual bool IsAssemblyResolutionInhibited(Assembly? requestingAssembly) => false;

    protected virtual Assembly LoadAssembly(string filePath, AssemblyName name) => AssemblyLoadPal.LoadFrom(filePath);

    public string? ResolveAssemblyPath(AssemblyName assemblyName)
    {
        if (m_ResolvedAssembliesCache.TryGetValue(assemblyName.FullName, out var assembly))
            return assembly?.Location;

        var reference = assemblyName;

        foreach (var i in GetProbingList())
        {
            var definition = i.Value;
            if (ReferenceMatchesDefinition(reference, definition))
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

        foreach (var libraryNameVariation in LibraryNameVariation.Enumerate(unmanagedDllName, isRelativePath))
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
