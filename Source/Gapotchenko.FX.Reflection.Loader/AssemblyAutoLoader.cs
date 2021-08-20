using Gapotchenko.FX.Reflection.Loader;
using Gapotchenko.FX.Reflection.Loader.Backends;
using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            this(AssemblyLoadContexts.Current)
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
        /// Releases all resources used by the <see cref="AssemblyAutoLoader"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the default instance of <see cref="AssemblyAutoLoader"/>.
        /// The default instance handles the current app domain and/or the effective assembly load context depending on a host environment.
        /// </summary>
        public static IAssemblyAutoLoader Default => DefaultAssemblyAutoLoader.Instance;

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
                    m_AssemblyDescriptors.Add(assembly, new AssemblyDescriptor(assembly, additionalProbingPaths, m_AssemblyLoadPal, this));
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

        IEnumerable<IAssemblyLoaderBackend> AssemblyLoaderBackends
        {
            get
            {
                lock (m_AssemblyDescriptors)
                    foreach (var i in m_AssemblyDescriptors)
                        foreach (var j in i.Value.AssemblyLoaderBackends)
                            yield return j;

                lock (m_ProbingPathResolvers)
                    foreach (var i in m_ProbingPathResolvers)
                        yield return i.Value;
            }
        }

        /// <inheritdoc/>
        public string? ResolveAssemblyPath(AssemblyName assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));

            return AssemblyLoaderBackends
                .Select(x => x.ResolveAssemblyPath(assemblyName))
                .FirstOrDefault(x => x != null);
        }

        /// <inheritdoc/>
        public string? ResolveUnmanagedDllPath(string unmanagedDllName)
        {
            if (unmanagedDllName == null)
                throw new ArgumentNullException(nameof(unmanagedDllName));

            return AssemblyLoaderBackends
                .Select(x => x.ResolveUnmanagedDllPath(unmanagedDllName))
                .FirstOrDefault(x => x != null);
        }
    }
}
