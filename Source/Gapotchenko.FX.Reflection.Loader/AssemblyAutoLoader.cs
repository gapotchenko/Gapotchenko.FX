using System;
using System.Reflection;

namespace Gapotchenko.FX.Reflection
{
    /// <summary>
    /// Provides global services for automatic assembly resolution and dynamic loading based on specified probing paths, binding redirects and common sense heuristics.
    /// </summary>
    public static class AssemblyAutoLoader
    {
        static readonly ScopedAssemblyAutoLoader m_Global = new ScopedAssemblyAutoLoader();

        /// <summary>
        /// Adds a specified assembly to the list of sources to consider during assembly resolution process for the current app domain.
        /// Once added, the loader automatically handles binding redirects according to a corresponding assembly configuration (<c>.config</c>) file.
        /// If configuration file is missing then binding redirects are automatically deducted according to the assembly compatibility heuristics.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if the assembly is added; <c>false</c> if the assembly is already added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <c>null</c>.</exception>
        public static bool AddAssembly(Assembly assembly) => m_Global.AddAssembly(assembly);

        /// <summary>
        /// Adds a specified assembly to the list of sources to consider during assembly resolution process for the current app domain.
        /// Once added, the loader automatically handles binding redirects according to a corresponding assembly configuration (<c>.config</c>) file.
        /// If configuration file is missing then binding redirects are automatically deducted according to the assembly compatibility heuristics.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="additionalProbingPaths">The additional probing paths for dependencies of a specified assembly.</param>
        /// <returns><c>true</c> if the assembly with the specified set of additional probing paths is added; <c>false</c> if the assembly with the specified set of additional probing paths is already added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <c>null</c>.</exception>
        public static bool AddAssembly(Assembly assembly, params string[]? additionalProbingPaths) => m_Global.AddAssembly(assembly, additionalProbingPaths);

        /// <summary>
        /// Removes a specified assembly from the list of sources to consider during assembly resolution process for the current app domain.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if the assembly is removed; <c>false</c> if the assembly already removed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <c>null</c>.</exception>
        public static bool RemoveAssembly(Assembly assembly) => m_Global.RemoveAssembly(assembly);

        /// <summary>
        /// Adds a specified probing path for the current app domain.
        /// Once added, establishes the specified directory path as the location of assemblies to probe during assembly resolution process.
        /// </summary>
        /// <param name="path">The probing path.</param>
        /// <returns><c>true</c> if the probing path is added; <c>false</c> if the probing path is already added.</returns>
        public static bool AddProbingPath(string path) => m_Global.AddProbingPath(path);

        /// <summary>
        /// Removes a specified probing path for the current app domain.
        /// Once removed, ceases to treat the specified directory path as the location of assemblies to probe during assembly resolution process.
        /// </summary>
        /// <param name="path">The probing path.</param>
        /// <returns><c>true</c> if the probing path is removed; <c>false</c> if the probing path is already removed.</returns>
        public static bool RemoveProbingPath(string path) => m_Global.RemoveProbingPath(path);
    }
}
