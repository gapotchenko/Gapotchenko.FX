using Gapotchenko.FX.Reflection.Loader;
using Gapotchenko.FX.Reflection.Loader.Backends;
using Gapotchenko.FX.Reflection.Loader.Pal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#nullable enable

namespace Gapotchenko.FX.Reflection
{
    /// <summary>
    /// Provides services with a controlled lifespan for automatic assembly resolution and dynamic loading based on specified probing paths, binding redirects and common sense heuristics.
    /// </summary>
    public sealed class ScopedAssemblyAutoLoader : IAssemblyAutoLoader, IDisposable
    {
        readonly Dictionary<Assembly, AssemblyDescriptor> m_AssemblyDescriptors = new Dictionary<Assembly, AssemblyDescriptor>();

        /// <summary>
        /// Adds a specified assembly to the list of sources to consider during assembly resolution process for the current app domain.
        /// Once added, the loader automatically handles binding redirects according to a corresponding assembly configuration (<c>.config</c>) file.
        /// If configuration file is missing then binding redirects are automatically deducted according to the assembly compatibility heuristics.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if the assembly is added; <c>false</c> if the assembly is already added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <c>null</c>.</exception>
        public bool AddAssembly(Assembly assembly) => AddAssembly(assembly, null);

        /// <summary>
        /// Adds a specified assembly to the list of sources to consider during assembly resolution process for the current app domain.
        /// Once added, the loader automatically handles binding redirects according to a corresponding assembly configuration (<c>.config</c>) file.
        /// If configuration file is missing then binding redirects are automatically deducted according to the assembly compatibility heuristics.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="additionalProbingPaths">The additional probing paths for dependencies of a specified assembly.</param>
        /// <returns><c>true</c> if the assembly with the specified set of additional probing paths is added; <c>false</c> if the assembly with the specified set of additional probing paths is already added.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <c>null</c>.</exception>
        public bool AddAssembly(Assembly assembly, params string[]? additionalProbingPaths)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            lock (m_AssemblyDescriptors)
            {
                EnsureNotDisposed();

                if (m_AssemblyDescriptors.TryGetValue(assembly, out var descriptor))
                {
                    return descriptor.AddProbingPaths(additionalProbingPaths);
                }
                else
                {
                    m_AssemblyDescriptors.Add(assembly, new AssemblyDescriptor(assembly, additionalProbingPaths));
                    return true;
                }
            }
        }

        /// <summary>
        /// Removes a specified assembly from the list of sources to consider during assembly resolution process for the current app domain.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if the assembly is removed; <c>false</c> if the assembly already removed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <c>null</c>.</exception>
        public bool RemoveAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            AssemblyDescriptor? descriptor;
            lock (m_AssemblyDescriptors)
                if (!m_AssemblyDescriptors.TryGetValue(assembly, out descriptor))
                    return false;

            descriptor.Dispose();
            return true;
        }

        readonly Dictionary<string, ProbingPathAssemblyLoaderBackend> m_ProbingPathResolvers = new Dictionary<string, ProbingPathAssemblyLoaderBackend>(FileSystem.PathComparer);

        /// <summary>
        /// Adds a specified probing path for the current app domain.
        /// Once added, establishes the specified directory path as the location of assemblies to probe during assembly resolution process.
        /// </summary>
        /// <param name="path">The probing path.</param>
        /// <returns><c>true</c> if the probing path is added; <c>false</c> if the probing path is already added.</returns>
        public bool AddProbingPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            path = Path.GetFullPath(path);

            lock (m_ProbingPathResolvers)
            {
                EnsureNotDisposed();

                if (m_ProbingPathResolvers.ContainsKey(path))
                    return false;

                m_ProbingPathResolvers.Add(path, new ProbingPathAssemblyLoaderBackend(path));
                return true;
            }
        }

        /// <summary>
        /// Removes a specified probing path for the current app domain.
        /// Once removed, ceases to treat the specified directory path as the location of assemblies to probe during assembly resolution process.
        /// </summary>
        /// <param name="path">The probing path.</param>
        /// <returns><c>true</c> if the probing path is removed; <c>false</c> if the probing path is already removed.</returns>
        public bool RemoveProbingPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            path = Path.GetFullPath(path);

            ProbingPathAssemblyLoaderBackend? loader;
            lock (m_ProbingPathResolvers)
                if (!m_ProbingPathResolvers.TryGetValue(path, out loader))
                    return false;

            loader.Dispose();
            return true;
        }

        bool m_Disposed;

        void EnsureNotDisposed()
        {
            if (m_Disposed)
                throw new ObjectDisposedException(null);
        }

        /// <summary>
        /// Unregisters all added probing paths and assemblies.
        /// Releases all resources used by the <see cref="ScopedAssemblyAutoLoader"/>.
        /// </summary>
        public void Dispose()
        {
            var disposables = new List<IDisposable>();

            lock (m_AssemblyDescriptors)
            {
                disposables.AddRange(m_AssemblyDescriptors.Values);
                m_AssemblyDescriptors.Clear();
                m_Disposed = true;
            }

            lock (m_ProbingPathResolvers)
            {
                disposables.AddRange(m_ProbingPathResolvers.Values);
                m_ProbingPathResolvers.Clear();
                m_Disposed = true;
            }

            foreach (var i in disposables)
                i.Dispose();
        }
    }
}
