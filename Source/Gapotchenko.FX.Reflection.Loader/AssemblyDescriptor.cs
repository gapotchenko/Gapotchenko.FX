using Gapotchenko.FX.Reflection.Loader.Backends;
using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader;

sealed class AssemblyDescriptor : IDisposable
{
    public AssemblyDescriptor(Assembly assembly, IEnumerable<string?>? additionalProbingPaths, AssemblyLoadPal assemblyLoadPal, AssemblyAutoLoader assemblyAutoLoader)
    {
        m_IsAttached = assemblyAutoLoader.IsAttached;
        m_AssemblyLoadPal = assemblyLoadPal;
        m_AssemblyDependencyTracker = new AssemblyDependencyTracker(assembly);

        string assemblyFilePath = new Uri(assembly.EscapedCodeBase).LocalPath;

        if (BindingRedirectAssemblyLoaderBackend.TryCreate(assemblyFilePath, assemblyAutoLoader, m_AssemblyLoadPal, m_AssemblyDependencyTracker, out m_AssemblyLoaderBackend))
        {
            m_HasBindingRedirects = m_AssemblyLoaderBackend != null;
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

            m_AssemblyLoaderBackend = new HeuristicAssemblyLoaderBackend(m_IsAttached, m_AssemblyLoadPal, m_AssemblyDependencyTracker, probingPaths.ToArray());
        }
    }

    readonly bool m_IsAttached;
    readonly AssemblyLoadPal m_AssemblyLoadPal;
    readonly AssemblyDependencyTracker m_AssemblyDependencyTracker;
    readonly IAssemblyLoaderBackend? m_AssemblyLoaderBackend;
    readonly bool m_HasBindingRedirects;

    HashSet<string>? m_ProbingPaths;
    List<HeuristicAssemblyLoaderBackend>? m_ProbingPathAssemblyLoaderBackends;

    public IEnumerable<IAssemblyLoaderBackend> AssemblyLoaderBackends
    {
        get
        {
            if (m_AssemblyLoaderBackend != null)
                yield return m_AssemblyLoaderBackend;

            if (m_ProbingPathAssemblyLoaderBackends != null)
                foreach (var i in m_ProbingPathAssemblyLoaderBackends)
                    yield return i;
        }
    }

    void _AccumulateNewProbingPaths(List<string> accumulator, IEnumerable<string?> probingPaths)
    {
        foreach (var i in probingPaths)
        {
            if (i == null)
                continue;

            string probingPath = Path.GetFullPath(i);

            m_ProbingPaths ??= new HashSet<string>(FileSystem.PathComparer);
            if (m_ProbingPaths.Add(probingPath))
                accumulator.Add(probingPath);
        }
    }

    public bool AddProbingPaths(IEnumerable<string?>? probingPaths)
    {
        if (probingPaths == null)
            return false;

        var newProbingPaths = new List<string>();
        _AccumulateNewProbingPaths(newProbingPaths, probingPaths);

        if (newProbingPaths.Count == 0)
            return false;

        m_ProbingPathAssemblyLoaderBackends ??= new();

        m_ProbingPathAssemblyLoaderBackends.Add(
            new HeuristicAssemblyLoaderBackend(m_IsAttached, m_AssemblyLoadPal, m_AssemblyDependencyTracker, newProbingPaths.ToArray())
            {
                StrictVersionMatch = m_HasBindingRedirects
            });

        return true;
    }

    public void Dispose()
    {
        m_AssemblyLoaderBackend?.Dispose();

        if (m_ProbingPathAssemblyLoaderBackends != null)
            foreach (var backend in m_ProbingPathAssemblyLoaderBackends)
                backend.Dispose();
    }
}
