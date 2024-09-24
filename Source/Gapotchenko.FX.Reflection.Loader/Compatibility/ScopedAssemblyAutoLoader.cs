// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

namespace Gapotchenko.FX.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides services with a controlled lifespan for automatic assembly resolution and dynamic loading based on specified probing paths, binding redirects and common sense heuristics.
/// </summary>
[Obsolete("Use AssemblyAutoLoader instead.")]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ScopedAssemblyAutoLoader : AssemblyAutoLoader
{
}
