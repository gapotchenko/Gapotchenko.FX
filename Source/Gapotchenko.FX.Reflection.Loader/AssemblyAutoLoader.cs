// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Reflection.Loader;
using Gapotchenko.FX.Reflection.Loader.Backends;
using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System.Reflection;
#if TFF_ASSEMBLYLOADCONTEXT
using System.Runtime.Loader;
#endif

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace Gapotchenko.FX.Reflection;

#if !BINARY_COMPATIBILITY
#pragma warning disable CS0109 // Member does not hide an accessible member. The new keyword is not required.
#endif

/// <summary>
/// Provides services for automatic assembly resolution and dynamic loading based on specified probing paths, binding redirects, and common sense heuristics.
/// </summary>
#pragma warning disable CS3009 // Base type is not CLS-compliant
public class AssemblyAutoLoader :
#pragma warning restore CS3009
#if BINARY_COMPATIBILITY
#pragma warning disable CS0618 // Type or member is obsolete
    _CompatibleAssemblyAutoLoader,
#pragma warning restore CS0618
#endif
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
    /// <param name="assemblyLoadContext">The assembly load context.</param>
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

#if TFF_ASSEMBLYLOADCONTEXT
    /// <summary>
    /// <para>
    /// Gets or configures the value indicating whether this <see cref="AssemblyAutoLoader"/> is attached
    /// to the associated <see cref="System.AppDomain"/> or <see cref="System.Runtime.Loader.AssemblyLoadContext"/>
    /// as an assembly resolution handler.
    /// </para>
    /// <para>
    /// The default value is <see langword="true"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The idea behind the attachment is that some <see cref="AssemblyAutoLoader"/> instances
    /// may be used just as standalone (i.e. non-attached) resolvers utilized only by code which calls
    /// <see cref="AssemblyAutoLoader.ResolveAssemblyPath(AssemblyName)"/> and
    /// <see cref="AssemblyAutoLoader.ResolveUnmanagedDllPath(string)"/>
    /// methods explicitly.
    /// </remarks>
#else
    /// <summary>
    /// <para>
    /// Gets or configures the value indicating whether this <see cref="AssemblyAutoLoader"/> is attached
    /// to the associated <see cref="System.AppDomain"/>
    /// as an assembly resolution handler.
    /// </para>
    /// <para>
    /// The default value is <see langword="true"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The idea behind the attachment is that some <see cref="AssemblyAutoLoader"/> instances
    /// may be used just as standalone (i.e. non-attached) resolvers utilized only by code which calls
    /// <see cref="AssemblyAutoLoader.ResolveAssemblyPath(AssemblyName)"/> and
    /// <see cref="AssemblyAutoLoader.ResolveUnmanagedDllPath(string)"/>
    /// methods explicitly.
    /// </remarks>
#endif
    public bool IsAttached { get; init; } = true;

#if TFF_ASSEMBLYLOADCONTEXT
    /// <summary>
    /// Gets the default instance of <see cref="AssemblyAutoLoader"/>.
    /// The default instance handles the current app domain and <see cref="System.Runtime.Loader.AssemblyLoadContext"/>.
    /// </summary>
#else
    /// <summary>
    /// Gets the default instance of <see cref="AssemblyAutoLoader"/>.
    /// The default instance handles the current app domain.
    /// </summary>
#endif
    public static AssemblyAutoLoader Default => DefaultAssemblyAutoLoader.Instance;

    readonly AssemblyLoadPal m_AssemblyLoadPal;
    readonly Dictionary<Assembly, AssemblyDescriptor> m_AssemblyDescriptors = [];

    /// <summary>
    /// Gets the associated app domain.
    /// </summary>
    public AppDomain AppDomain => m_AssemblyLoadPal.AppDomain ?? AppDomain.CurrentDomain;

#if TFF_ASSEMBLYLOADCONTEXT
    /// <summary>
    /// Gets the associated assembly load context.
    /// </summary>
    public AssemblyLoadContext? AssemblyLoadContext => m_AssemblyLoadPal.AssemblyLoadContaxt;
#endif

    /// <summary>
    /// Adds the location of a specified assembly to the list of sources to consider during assembly resolution.
    /// Once added, the assembly loader automatically handles binding redirects according to the assembly configuration (<c>.config</c>) file.
    /// If configuration file does not exist then binding redirects are automatically deducted according to the assembly compatibility heuristics.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns><see langword="true"/> if the assembly is added; <see langword="false"/> if the assembly is already added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <see langword="null"/>.</exception>
    public new bool AddAssembly(Assembly assembly) => AddAssembly(assembly, (IEnumerable<string?>?)null);

    /// <summary>
    /// Adds the location of a specified assembly to the list of sources to consider during assembly resolution.
    /// Once added, the assembly loader automatically handles binding redirects according to the assembly configuration (<c>.config</c>) file.
    /// If configuration file does not exist then binding redirects are automatically deducted according to the assembly compatibility heuristics.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="additionalProbingPaths">The additional probing paths for dependencies of a specified assembly.</param>
    /// <returns><see langword="true"/> if the assembly with the specified set of additional probing paths is added; <see langword="false"/> if the assembly with the specified set of additional probing paths is already added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <see langword="null"/>.</exception>
    public new bool AddAssembly(Assembly assembly, params string?[]? additionalProbingPaths) =>
        AddAssembly(assembly, (IEnumerable<string?>?)additionalProbingPaths);

    /// <summary>
    /// Adds the location of a specified assembly to the list of sources to consider during assembly resolution.
    /// Once added, the assembly loader automatically handles binding redirects according to the assembly configuration (<c>.config</c>) file.
    /// If configuration file does not exist then binding redirects are automatically deducted according to the assembly compatibility heuristics.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="additionalProbingPaths">The additional probing paths for dependencies of a specified assembly.</param>
    /// <returns><see langword="true"/> if the assembly with the specified set of additional probing paths is added; <see langword="false"/> if the assembly with the specified set of additional probing paths is already added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <see langword="null"/>.</exception>
    public bool AddAssembly(Assembly assembly, IEnumerable<string?>? additionalProbingPaths)
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

    /// <summary>
    /// Removes the location of a specified assembly from the list of sources to consider during assembly resolution.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns><see langword="true"/> if the assembly is removed; <see langword="false"/> if the assembly already removed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> parameter is <see langword="null"/>.</exception>
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

    /// <summary>
    /// Adds a specified probing path.
    /// Once added, establishes the specified directory path as the location of assemblies to probe during assembly resolution.
    /// </summary>
    /// <param name="path">The probing path.</param>
    /// <returns><see langword="true"/> if the probing path is added; <see langword="false"/> if the probing path is already added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> parameter is <see langword="null"/>.</exception>
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

            m_ProbingPathResolvers.Add(path, new ProbingPathAssemblyLoaderBackend(IsAttached, m_AssemblyLoadPal, [path]));
            return true;
        }
    }

    /// <summary>
    /// Removes a specified probing path.
    /// Once removed, ceases to treat the specified directory path as the location of assemblies to probe during assembly resolution.
    /// </summary>
    /// <param name="path">The probing path.</param>
    /// <returns><see langword="true"/> if the probing path is removed; <see langword="false"/> if the probing path is already removed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> parameter is <see langword="null"/>.</exception>
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

    /// <summary>
    /// Resolves the file path of an assembly with specified name.
    /// </summary>
    /// <param name="assemblyName">The assembly name.</param>
    /// <returns>The assembly file path or <see langword="null"/> if the assembly is unresolved.</returns>
    public string? ResolveAssemblyPath(AssemblyName assemblyName)
    {
        if (assemblyName == null)
            throw new ArgumentNullException(nameof(assemblyName));

        return AssemblyLoaderBackends
            .Select(x => x.ResolveAssemblyPath(assemblyName))
            .FirstOrDefault(x => x != null);
    }

    /// <summary>
    /// Resolves the file path of an unmanaged DLL with specified name.
    /// </summary>
    /// <param name="unmanagedDllName">The name of unmanaged DLL.</param>
    /// <returns>The DLL file path or <see langword="null"/> if the DLL is unresolved.</returns>
    public string? ResolveUnmanagedDllPath(string unmanagedDllName)
    {
        if (unmanagedDllName == null)
            throw new ArgumentNullException(nameof(unmanagedDllName));

        return AssemblyLoaderBackends
            .Select(x => x.ResolveUnmanagedDllPath(unmanagedDllName))
            .FirstOrDefault(x => x != null);
    }

    void EnsureNotDisposed()
    {
        if (m_Disposed)
            throw new ObjectDisposedException(null);
    }

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
    /// Releases the unmanaged resources and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release both managed and unmanaged resources;
    /// <see langword="false"/> to release only unmanaged resources.
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

    bool m_Disposed;
}
