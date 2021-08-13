﻿using Gapotchenko.FX.Reflection.Loader;
using Gapotchenko.FX.Reflection.Loader.Backends;
using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
#if TFF_ASSEMBLYLOADCONTEXT
using System.Runtime.Loader;
#endif

namespace Gapotchenko.FX.Reflection
{
    /// <summary>
    /// Provides services for automatic assembly resolution and dynamic loading based on specified probing paths, binding redirects, and common sense heuristics.
    /// </summary>
#pragma warning disable CS3009
    public class AssemblyAutoLoader :
#pragma warning restore CS3009
#pragma warning disable CS0618
        _CompatibleAssemblyAutoLoader,
#pragma warning restore CS0618
        IAssemblyAutoLoader,
        IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyAutoLoader"/> class.
        /// </summary>
#if TFF_ASSEMBLYLOADCONTEXT
        public AssemblyAutoLoader() :
            this(AssemblyLoadContexts.Effective)
        {
        }
#else
        public AssemblyAutoLoader()
        {
            m_AssemblyLoadPal = AssemblyLoadPal.Default;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyAutoLoader"/> class for the specified app domain.
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        public AssemblyAutoLoader(AppDomain appDomain)
        {
            if (appDomain == null)
                throw new ArgumentNullException(nameof(appDomain));

            if (appDomain == AppDomain.CurrentDomain)
                m_AssemblyLoadPal = AssemblyLoadPal.Default;
            else
                m_AssemblyLoadPal = new AssemblyLoadPal(appDomain);
        }

#if TFF_ASSEMBLYLOADCONTEXT
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyAutoLoader"/> class for the specified assembly load context.
        /// </summary>
        /// <param name="assemblyLoadContext">The app domain.</param>
        public AssemblyAutoLoader(AssemblyLoadContext assemblyLoadContext)
        {
            if (assemblyLoadContext == null)
                throw new ArgumentNullException(nameof(assemblyLoadContext));

            if (assemblyLoadContext == AssemblyLoadContext.Default)
                m_AssemblyLoadPal = AssemblyLoadPal.Default;
            else
                m_AssemblyLoadPal = new AssemblyLoadPal(assemblyLoadContext);
        }
#endif

        /// <summary>
        /// Finalizes the instance of <see cref="AssemblyAutoLoader"/> class.
        /// </summary>
        ~AssemblyAutoLoader()
        {
            Dispose(false);
        }

        /// <summary>
        /// Unregisters all added probing paths and assemblies.
        /// Releases all resources used by the <see cref="ScopedAssemblyAutoLoader"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#if TFF_ASSEMBLYLOADCONTEXT
        /// <summary>
        /// Gets <see cref="AssemblyAutoLoader"/> instance for the default <see cref="AssemblyLoadContext"/>.
        /// </summary>
#else
        /// <summary>
        /// Gets default <see cref="AssemblyAutoLoader"/> instance for the current app domain.
        /// </summary>
#endif
        public static AssemblyAutoLoader Default => DefaultAssemblyAutoLoader.Instance;

        readonly AssemblyLoadPal m_AssemblyLoadPal;
        readonly Dictionary<Assembly, AssemblyDescriptor> m_AssemblyDescriptors = new();

        /// <inheritdoc/>
        public new bool AddAssembly(Assembly assembly) => AddAssembly(assembly, null);

        /// <inheritdoc/>
        public new bool AddAssembly(Assembly assembly, params string?[]? additionalProbingPaths)
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
                    m_AssemblyDescriptors.Add(assembly, new AssemblyDescriptor(assembly, additionalProbingPaths, m_AssemblyLoadPal));
                    return true;
                }
            }
        }

        /// <inheritdoc/>
        public new bool RemoveAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            AssemblyDescriptor? descriptor;
            lock (m_AssemblyDescriptors)
                if (!m_AssemblyDescriptors.Remove(assembly, out descriptor))
                    return false;

            descriptor.Dispose();
            return true;
        }

        readonly Dictionary<string, ProbingPathAssemblyLoaderBackend> m_ProbingPathResolvers = new(FileSystem.PathComparer);

        /// <inheritdoc/>
        public new bool AddProbingPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            path = Path.GetFullPath(path);

            lock (m_ProbingPathResolvers)
            {
                EnsureNotDisposed();

                if (m_ProbingPathResolvers.ContainsKey(path))
                    return false;

                m_ProbingPathResolvers.Add(path, new ProbingPathAssemblyLoaderBackend(m_AssemblyLoadPal, path));
                return true;
            }
        }

        /// <inheritdoc/>
        public new bool RemoveProbingPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            path = Path.GetFullPath(path);

            ProbingPathAssemblyLoaderBackend? loader;
            lock (m_ProbingPathResolvers)
                if (!m_ProbingPathResolvers.Remove(path, out loader))
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
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            var disposables = new List<IDisposable>();

            lock (m_AssemblyDescriptors)
            {
                disposables.AddRange(m_AssemblyDescriptors.Values);
                m_AssemblyDescriptors.Clear();
            }

            lock (m_ProbingPathResolvers)
            {
                disposables.AddRange(m_ProbingPathResolvers.Values);
                m_ProbingPathResolvers.Clear();
            }

            m_Disposed = true;

            foreach (var i in disposables)
                i.Dispose();
        }
    }
}
