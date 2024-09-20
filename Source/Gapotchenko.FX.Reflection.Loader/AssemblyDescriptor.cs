using Gapotchenko.FX.Reflection.Loader.Backends;
using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader;

sealed class AssemblyDescriptor : IDisposable
{
    public AssemblyDescriptor(
        Assembly assembly,
        IEnumerable<string?>? additionalProbingPaths,
        AssemblyLoadPal assemblyLoadPal,
        AssemblyAutoLoader assemblyAutoLoader)
    {
        m_IsAttached = assemblyAutoLoader.IsAttached;
        m_AssemblyLoadPal = assemblyLoadPal;
        m_AssemblyDependencyTracker = new AssemblyDependencyTracker(assembly);

        string assemblyFilePath =
#if NET5_0_OR_GREATER
            assembly.Location;
#else
            new Uri(assembly.EscapedCodeBase).LocalPath;
#endif

        string? assemblyDirectoryPath = Path.GetDirectoryName(assemblyFilePath);

        if (BindingRedirectAssemblyLoaderBackend.TryCreate(
            assemblyFilePath,
            assemblyAutoLoader,
            m_AssemblyLoadPal,
            m_AssemblyDependencyTracker,
            out m_AssemblyLoaderBackend,
            out var bindingProbingPaths))
        {
            m_HasBindingRedirects = m_AssemblyLoaderBackend != null;

            var probingPaths = new List<string>();
            if (bindingProbingPaths != null)
                probingPaths.AddRange(bindingProbingPaths);
            if (!string.IsNullOrEmpty(assemblyDirectoryPath))
                probingPaths.Add(assemblyDirectoryPath);
            AddProbingPaths(probingPaths);

            AddProbingPaths(additionalProbingPaths);
        }
        else
        {
            var probingPaths = new List<string>();

            if (!string.IsNullOrEmpty(assemblyDirectoryPath))
                probingPaths.Add(assemblyDirectoryPath);

            if (additionalProbingPaths != null)
                AccumulateNewProbingPaths(probingPaths, additionalProbingPaths);

            m_AssemblyLoaderBackend = new HeuristicAssemblyLoaderBackend(
                m_IsAttached,
                m_AssemblyLoadPal,
                m_AssemblyDependencyTracker,
                probingPaths.ToArray());
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

    void AccumulateNewProbingPaths(List<string> accumulator, IEnumerable<string?> probingPaths)
    {
        foreach (var i in probingPaths)
        {
            if (i == null)
                continue;

            string probingPath = Path.GetFullPath(i);

            if ((m_ProbingPaths ??= new(FileSystem.PathComparer)).Add(probingPath))
                accumulator.Add(probingPath);
        }
    }

    public bool AddProbingPaths(IEnumerable<string?>? probingPaths)
    {
        if (probingPaths == null)
            return false;

        var newProbingPaths = new List<string>();
        AccumulateNewProbingPaths(newProbingPaths, probingPaths);

        if (newProbingPaths.Count == 0)
            return false;

        (m_ProbingPathAssemblyLoaderBackends ??= [])
        .Add(
            new HeuristicAssemblyLoaderBackend(
                m_IsAttached,
                m_AssemblyLoadPal,
                m_AssemblyDependencyTracker,
                newProbingPaths.ToArray())
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
