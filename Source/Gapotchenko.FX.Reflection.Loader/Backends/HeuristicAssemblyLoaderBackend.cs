using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Backends;

sealed class HeuristicAssemblyLoaderBackend(
    bool isAttached,
    AssemblyLoadPal assemblyLoadPal,
    AssemblyDependencyTracker assemblyDependencyTracker,
    IReadOnlyList<string> probingPaths) :
    ProbingPathAssemblyLoaderBackend(isAttached, assemblyLoadPal, probingPaths)
{
    protected override bool IsAssemblyResolutionInhibited(Assembly? requstingAssembly) =>
        assemblyDependencyTracker.IsAssemblyResolutionInhibited(requstingAssembly);

    protected override Assembly LoadAssembly(string filePath, AssemblyName name)
    {
        bool registered = assemblyDependencyTracker.RegisterReferencedAssembly(name);
        try
        {
            return base.LoadAssembly(filePath, name);
        }
        catch
        {
            if (registered)
                assemblyDependencyTracker.UnregisterReferencedAssembly(name);
            throw;
        }
    }
}
