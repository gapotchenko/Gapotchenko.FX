using System.ComponentModel;
using System.Reflection;

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
    // This class is needed for binary compatibility.

    /// <summary>
    /// Adds a specified assembly to the list of sources to consider during assembly resolution process for the current app domain.
    /// Once added, the loader automatically handles binding redirects according to a corresponding assembly configuration (<c>.config</c>) file.
    /// If configuration file is missing then binding redirects are automatically deducted according to the assembly compatibility heuristics.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns><c>true</c> if the assembly is added; <c>false</c> if the assembly is already added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <c>null</c>.</exception>
    [Obsolete("Use AssemblyAutoLoader.Default.AddAssembly instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool AddAssembly(Assembly assembly) => AssemblyAutoLoader.Default.AddAssembly(assembly);

    /// <summary>
    /// Adds a specified assembly to the list of sources to consider during assembly resolution process for the current app domain.
    /// Once added, the loader automatically handles binding redirects according to a corresponding assembly configuration (<c>.config</c>) file.
    /// If configuration file is missing then binding redirects are automatically deducted according to the assembly compatibility heuristics.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="additionalProbingPaths">The additional probing paths for dependencies of a specified assembly.</param>
    /// <returns><c>true</c> if the assembly with the specified set of additional probing paths is added; <c>false</c> if the assembly with the specified set of additional probing paths is already added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <c>null</c>.</exception>
    [Obsolete("Use AssemblyAutoLoader.Default.AddAssembly instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool AddAssembly(Assembly assembly, params string?[]? additionalProbingPaths) => AssemblyAutoLoader.Default.AddAssembly(assembly, additionalProbingPaths);

    /// <summary>
    /// Removes a specified assembly from the list of sources to consider during assembly resolution process for the current app domain.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns><c>true</c> if the assembly is removed; <c>false</c> if the assembly already removed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <c>null</c>.</exception>
    [Obsolete("Use AssemblyAutoLoader.Default.RemoveAssembly instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool RemoveAssembly(Assembly assembly) => AssemblyAutoLoader.Default.RemoveAssembly(assembly);

    /// <summary>
    /// Adds a specified probing path for the current app domain.
    /// Once added, establishes the specified directory path as the location of assemblies to probe during assembly resolution process.
    /// </summary>
    /// <param name="path">The probing path.</param>
    /// <returns><c>true</c> if the probing path is added; <c>false</c> if the probing path is already added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> parameter is <c>null</c>.</exception>
    [Obsolete("Use AssemblyAutoLoader.Default.AddProbingPath instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool AddProbingPath(string path) => AssemblyAutoLoader.Default.AddProbingPath(path);

    /// <summary>
    /// Removes a specified probing path for the current app domain.
    /// Once removed, ceases to treat the specified directory path as the location of assemblies to probe during assembly resolution process.
    /// </summary>
    /// <param name="path">The probing path.</param>
    /// <returns><c>true</c> if the probing path is removed; <c>false</c> if the probing path is already removed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> parameter is <c>null</c>.</exception>
    [Obsolete("Use AssemblyAutoLoader.Default.RemoveProbingPath instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool RemoveProbingPath(string path) => AssemblyAutoLoader.Default.RemoveProbingPath(path);
}
