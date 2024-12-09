// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

#if BINARY_COMPATIBILITY

using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace Gapotchenko.FX.Reflection;

/// <summary>
/// Infrastructure.
/// Should not be used in user code.
/// </summary>
[Obsolete("Should not be used in user code.")]
[CLSCompliant(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class _CompatibleAssemblyAutoLoader
{
    /// <inheritdoc cref="AssemblyAutoLoader.AddAssembly(Assembly)"/>
    [Obsolete("Use AssemblyAutoLoader.Default.AddAssembly instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool AddAssembly(Assembly assembly) => AssemblyAutoLoader.Default.AddAssembly(assembly);

    /// <inheritdoc cref="AssemblyAutoLoader.AddAssembly(Assembly, string[])"/>
    [Obsolete("Use AssemblyAutoLoader.Default.AddAssembly instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool AddAssembly(Assembly assembly, params string?[]? additionalProbingPaths) => AssemblyAutoLoader.Default.AddAssembly(assembly, additionalProbingPaths);

    /// <inheritdoc cref="AssemblyAutoLoader.RemoveAssembly(Assembly)"/>
    [Obsolete("Use AssemblyAutoLoader.Default.RemoveAssembly instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool RemoveAssembly(Assembly assembly) => AssemblyAutoLoader.Default.RemoveAssembly(assembly);

    /// <inheritdoc cref="AssemblyAutoLoader.AddProbingPath(string)"/>
    [Obsolete("Use AssemblyAutoLoader.Default.AddProbingPath instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool AddProbingPath(string path) => AssemblyAutoLoader.Default.AddProbingPath(path);

    /// <inheritdoc cref="AssemblyAutoLoader.RemoveProbingPath(string)"/>
    [Obsolete("Use AssemblyAutoLoader.Default.RemoveProbingPath instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool RemoveProbingPath(string path) => AssemblyAutoLoader.Default.RemoveProbingPath(path);
}

#endif
