// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader;

/// <summary>
/// This class allows for an assembly loader to be as lazy as possible.
/// </summary>
/// <remarks>
/// <para>
/// For example, there is a subtle bug in initialization of System.Net.ServicePointManager
/// class that presumably occurs when .NET configuration manager is accessed
/// from a module initializer (as of .NET Framework 4.8.1). One way to hit the
/// bug is to call System.Xml.XDocument.Load(string) method from a module
/// initializer.
/// </para>
/// <para>
/// For some mysterious reason, the SecurityProtocol property of
/// System.Net.ServicePointManager class is then set to a bad value in that
/// case, leading a catastrophic Internet communication failure inside the
/// affected .NET app domain.
/// </para>
/// <para>
/// There is a reasonable assumption to believe that the situations like that
/// may be way more widespread in the wild because module initialization
/// is a relatively niche feature.
/// </para>
/// </remarks>
sealed class AssemblyLoaderInitializer : IDisposable
{
    public AssemblyLoaderInitializer(AssemblyLoadPal assemblyLoadPal)
    {
        if (!m_Flushed)
        {
            m_AssemblyLoadPal = assemblyLoadPal;
            m_Actions = [];

            assemblyLoadPal.Resolving += AssemblyLoadPal_Resolving;
        }
    }

    public IAssemblyLoaderInitializationScope CreateScope() =>
        m_Flushed ? DirectScope.Instance : new Scope(this);

    sealed class DirectScope : IAssemblyLoaderInitializationScope
    {
        public static DirectScope Instance { get; } = new();

        public void Enqueue(Action action) => action();
        public void Flush() { }
        public void Dispose() { }
    }

    sealed class Scope(AssemblyLoaderInitializer initializer) : IAssemblyLoaderInitializationScope
    {
        public void Enqueue(Action action) => initializer.Enqueue(action, this);
        public void Flush() => initializer.Flush();
        public void Dispose() => initializer.Discard(this);
    }

    void Enqueue(Action action, IAssemblyLoaderInitializationScope scope)
    {
        lock (this)
        {
            var actions = m_Actions;
            if (actions != null)
            {
                actions.Add(new() { Delegate = action, Scope = scope });
                return;
            }
        }

        action();
    }

    void Discard(IAssemblyLoaderInitializationScope? scope)
    {
        lock (this)
            m_Actions?.RemoveAll(x => x.Scope == scope);
    }

    void Flush()
    {
        List<ActionDescriptor>? actions;

        lock (this)
        {
            actions = m_Actions;
            if (actions == null)
            {
                // Already flushed or disposed.
                return;
            }
            m_Actions = null;
        }

        m_Flushed = true;
        Unsubscribe();

        foreach (var action in actions)
            action.Delegate();
    }

    /// <summary>
    /// Signifies whether an assembly loader initialization was performed at
    /// least once.
    /// </summary>
    /// <remarks>
    /// Represented by a static field because the issues lazy initialization
    /// tries to workaround have a scope of an OS process.
    /// </remarks>
    static bool m_Flushed;

    public void Dispose()
    {
        m_Actions = null;
        Unsubscribe();
    }

    List<ActionDescriptor>? m_Actions;

    readonly struct ActionDescriptor
    {
        public required Action Delegate { get; init; }
        public IAssemblyLoaderInitializationScope? Scope { get; init; }
    }

    void Unsubscribe()
    {
        var assemblyLoadPal = m_AssemblyLoadPal;
        if (assemblyLoadPal != null)
        {
            assemblyLoadPal.Resolving -= AssemblyLoadPal_Resolving;
            m_AssemblyLoadPal = null;
        }
    }

    AssemblyLoadPal? m_AssemblyLoadPal;

    Assembly? AssemblyLoadPal_Resolving(AssemblyLoadPal sender, AssemblyLoadPal.ResolvingEventArgs args)
    {
        Flush();
        return null;
    }
}
