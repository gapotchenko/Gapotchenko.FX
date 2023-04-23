using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Backends;

sealed class HeuristicAssemblyLoaderBackend : ProbingPathAssemblyLoaderBackend
{
    public HeuristicAssemblyLoaderBackend(
        bool isAttached,
        AssemblyLoadPal assemblyLoadPal,
        AssemblyDependencyTracker assemblyDependencyTracker,
        IReadOnlyList<string> probingPaths) :
        base(isAttached, assemblyLoadPal, probingPaths)
    {
        _AssemblyDependencyTracker = assemblyDependencyTracker;
    }

    readonly AssemblyDependencyTracker _AssemblyDependencyTracker;

    protected override bool IsAssemblyResolutionInhibited(Assembly? requstingAssembly) =>
        _AssemblyDependencyTracker.IsAssemblyResolutionInhibited(requstingAssembly);

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
