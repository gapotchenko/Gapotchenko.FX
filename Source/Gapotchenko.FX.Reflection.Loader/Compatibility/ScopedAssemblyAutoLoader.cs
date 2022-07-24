using System.ComponentModel;

namespace Gapotchenko.FX.Reflection;

/// <summary>
/// Provides services with a controlled lifespan for automatic assembly resolution and dynamic loading based on specified probing paths, binding redirects and common sense heuristics.
/// </summary>
[Obsolete("Use AssemblyAutoLoader instead.")]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ScopedAssemblyAutoLoader : AssemblyAutoLoader
{
}
