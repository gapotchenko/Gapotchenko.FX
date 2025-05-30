﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Diagnostics;
using System.Reflection;
#if TFF_ASSEMBLYLOADCONTEXT
using System.Runtime.Loader;
#endif

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace Gapotchenko.FX.Reflection;

/// <summary>
/// Platform abstraction layer for assembly loading functionality of a .NET host environment.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class AssemblyLoadPal
{
    /// <summary>
    /// Gets the instance of <see cref="AssemblyLoadPal"/> for the current app domain.
    /// </summary>
    public static AssemblyLoadPal ForCurrentAppDomain { get; } = new AssemblyLoadPal(AppDomain.CurrentDomain);

    /// <summary>
    /// Gets the default instance of <see cref="AssemblyLoadPal"/>.
    /// The default instance handles the current app domain and/or the default assembly load context depending on a host environment.
    /// </summary>
    public static AssemblyLoadPal Default { get; } =
#if TFF_ASSEMBLYLOADCONTEXT
        new AssemblyLoadPal(AppDomain.CurrentDomain, AssemblyLoadContext.Default);
#else
        ForCurrentAppDomain;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyLoadPal"/> class for the specified app domain.
    /// </summary>
    public AssemblyLoadPal(AppDomain appDomain)
    {
        if (appDomain == null)
            throw new ArgumentNullException(nameof(appDomain));

        m_AppDomain = appDomain;
    }

#if TFF_ASSEMBLYLOADCONTEXT
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyLoadPal"/> class for the specified assembly load context.
    /// </summary>
    public AssemblyLoadPal(AssemblyLoadContext assemblyLoadContext)
    {
        if (assemblyLoadContext == null)
            throw new ArgumentNullException(nameof(assemblyLoadContext));

        m_AssemblyLoadContext = assemblyLoadContext;
    }

    AssemblyLoadPal(AppDomain appDomain, AssemblyLoadContext assemblyLoadContext)
    {
        m_AppDomain = appDomain;
        m_AssemblyLoadContext = assemblyLoadContext;
    }
#endif

    /// <summary>
    /// Occurs when the resolution of an assembly fails.
    /// </summary>
    public event ResolvingEventHandler Resolving
    {
        add
        {
            lock (m_Lock)
            {
                bool was = m_Resolving != null;
                m_Resolving += value;
                if (!was && m_Resolving != null)
                    SetupResolving();
            }
        }
        remove
        {
            lock (m_Lock)
            {
                bool was = m_Resolving != null;
                m_Resolving -= value;
                if (was && m_Resolving == null)
                    TeardownResolving();
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly object m_Lock = new();

    void SetupResolving()
    {
        if (m_AppDomain != null)
            m_AppDomain.AssemblyResolve += AppDomain_AssemblyResolve;

#if TFF_ASSEMBLYLOADCONTEXT
        if (m_AppDomain == null && m_AssemblyLoadContext != null)
            m_AssemblyLoadContext.Resolving += AssemblyLoadContext_Resolving;
#endif
    }

    void TeardownResolving()
    {
#if TFF_ASSEMBLYLOADCONTEXT
        if (m_AppDomain == null && m_AssemblyLoadContext != null)
            m_AssemblyLoadContext.Resolving -= AssemblyLoadContext_Resolving;
#endif

        if (m_AppDomain != null)
            m_AppDomain.AssemblyResolve -= AppDomain_AssemblyResolve;
    }

    Assembly? AppDomain_AssemblyResolve(object? sender, ResolveEventArgs args) =>
        InvokeResolving(new ResolvingEventArgs(args.RequestingAssembly, args.Name!));

#if TFF_ASSEMBLYLOADCONTEXT
    Assembly? AssemblyLoadContext_Resolving(AssemblyLoadContext assemblyLoadContext, AssemblyName name) =>
        InvokeResolving(new ResolvingEventArgs(name));
#endif

    Assembly? InvokeResolving(ResolvingEventArgs args)
    {
        var eh = m_Resolving;
        if (eh != null)
        {
            var invocationList = eh.GetInvocationList();
            foreach (ResolvingEventHandler handler in invocationList)
            {
                var assembly = handler(this, args);
                if (assembly != null)
                    return assembly;

                var newEh = m_Resolving;
                if (newEh != null && newEh != eh)
                {
                    // The list of event handlers changed on the fly. Apply
                    // the changes by running the handlers continuously while
                    // the event subscription keeps on changing. This approach
                    // allows the handlers to dynamically unfold themselves on
                    // rare occasions when they prefer to do so.
                    return InvokeResolvingRare(
                        newEh,
                        args,
                        // Avoid duplicate handler invocations.
                        [.. invocationList.TakeWhile(x => !ReferenceEquals(x, handler)), handler]);
                }
            }
        }
        return null;
    }

    Assembly? InvokeResolvingRare(ResolvingEventHandler eh, ResolvingEventArgs args, HashSet<Delegate> invokedHandlers)
    {
    RunAgain:
        foreach (ResolvingEventHandler handler in eh.GetInvocationList())
        {
            if (!invokedHandlers.Add(handler))
                continue;

            var assembly = handler(this, args);
            if (assembly != null)
                return assembly;

            var newEh = m_Resolving;
            if (newEh != null && newEh != eh)
            {
                // The list of handlers changed on the fly.
                // Apply the changes.
                eh = newEh;
                goto RunAgain;
            }
        }
        return null;
    }

    ResolvingEventHandler? m_Resolving;

    /// <summary>
    /// Represents a method that handles the <see cref="Resolving"/> event of an <see cref="AssemblyLoadPal"/>.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>The resolved assembly or <see langword="null"/> if the assembly cannot be resolved.</returns>
    public delegate Assembly? ResolvingEventHandler(AssemblyLoadPal sender, ResolvingEventArgs args);

    /// <summary>
    /// Provides data for assembly loader resolution event.
    /// </summary>
    public sealed class ResolvingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolvingEventArgs"/> class.
        /// </summary>
        /// <param name="requestingAssembly">The assembly whose dependency is being resolved.</param>
        /// <param name="fullName">The full name of the assembly to resolve.</param>
        public ResolvingEventArgs(Assembly? requestingAssembly, string fullName)
        {
            RequestingAssembly = requestingAssembly;
            m_FullName = fullName;
        }

#if TFF_ASSEMBLYLOADCONTEXT
        internal ResolvingEventArgs(AssemblyName name)
        {
            m_Name = name;
        }
#endif

        /// <summary>
        /// Gets the assembly whose dependency is being resolved.
        /// </summary>
        public Assembly? RequestingAssembly { get; }

        /// <summary>
        /// Gets the full name of the assembly to resolve.
        /// </summary>
        public string FullName =>
#if TFF_ASSEMBLYLOADCONTEXT
            m_FullName ??= (m_Name ?? throw new InvalidOperationException()).FullName;
#else
            m_FullName;
#endif

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if TFF_ASSEMBLYLOADCONTEXT
        string? m_FullName;
#else
        readonly string m_FullName;
#endif

        /// <summary>
        /// Gets the strongly-typed name of the assembly to resolve.
        /// </summary>
        public AssemblyName Name => m_Name ??= new AssemblyName(m_FullName ?? throw new InvalidOperationException());

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        AssemblyName? m_Name;
    }

    /// <summary>
    /// Loads an assembly given its file path.
    /// </summary>
    /// <param name="assemblyFile">The assembly file path.</param>
    /// <returns>The loaded assembly.</returns>
    public Assembly LoadFrom(string assemblyFile)
    {
#if TFF_ASSEMBLYLOADCONTEXT
        if (m_AssemblyLoadContext != null)
            return m_AssemblyLoadContext.LoadFromAssemblyPath(assemblyFile);
#endif

        if (m_AppDomain != null)
        {
#if !TFF_SINGLE_APPDOMAIN
            if (m_AppDomain != AppDomain.CurrentDomain)
            {
                return m_AppDomain.Load(
                    new AssemblyName
                    {
                        CodeBase = assemblyFile
                    });
            }
#endif
            return Assembly.LoadFrom(assemblyFile);
        }

        // Never reached because m_AppDomain and m_AssemblyLoadContext cannot be null simultaneously.
        throw new InvalidOperationException();
    }

    /// <summary>
    /// Loads an assembly given its <see cref="AssemblyName"/>.
    /// </summary>
    /// <param name="assemblyName">The assembly name.</param>
    /// <returns>The loaded assembly.</returns>
    public Assembly Load(AssemblyName assemblyName)
    {
#if TFF_ASSEMBLYLOADCONTEXT
        if (m_AssemblyLoadContext != null)
            return m_AssemblyLoadContext.LoadFromAssemblyName(assemblyName);
#endif

        if (m_AppDomain != null)
        {
#if !TFF_SINGLE_APPDOMAIN
            if (m_AppDomain != AppDomain.CurrentDomain)
                return m_AppDomain.Load(assemblyName);
#endif
            return Assembly.Load(assemblyName);
        }

        // Never reached because m_AppDomain and m_AssemblyLoadContext cannot be null simultaneously.
        throw new InvalidOperationException();
    }

    /// <summary>
    /// Gets the associated app domain.
    /// </summary>
    public AppDomain? AppDomain => m_AppDomain;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AppDomain? m_AppDomain;

#if TFF_ASSEMBLYLOADCONTEXT
    /// <summary>
    /// Gets the associated assembly load context.
    /// </summary>
    public AssemblyLoadContext? AssemblyLoadContaxt => m_AssemblyLoadContext;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AssemblyLoadContext? m_AssemblyLoadContext;
#endif
}
