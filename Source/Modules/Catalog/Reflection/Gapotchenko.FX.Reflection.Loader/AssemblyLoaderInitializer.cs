// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using System.Diagnostics;
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
        if (!m_InitializationIsDone)
        {
            m_AssemblyLoadPal = assemblyLoadPal;
            m_Actions = [];

            // Become active on a first assembly resolution request.
            assemblyLoadPal.Resolving += AssemblyLoadPal_Resolving;
        }
    }

    public IAssemblyLoaderInitializationScope CreateScope() =>
        m_InitializationIsDone ? EagerScope.Instance : new LazyScope(this);

    /// <summary>
    /// Provides immediate execution.
    /// </summary>
    sealed class EagerScope : IAssemblyLoaderInitializationScope
    {
        public static EagerScope Instance { get; } = new();

        public bool IsLazy => false;
        public void Enqueue(Action action) => action();
        public void Flush() { }
        public void Dispose() { }
    }

    /// <summary>
    /// Provides deferred execution.
    /// </summary>
    sealed class LazyScope(AssemblyLoaderInitializer initializer) : IAssemblyLoaderInitializationScope
    {
        public bool IsLazy => !m_InitializationIsDone;
        public void Enqueue(Action action) => initializer.Enqueue(action, this);
        public void Flush() => initializer.Flush(this);
        public void Dispose() => initializer.Discard(this);
    }

    void Enqueue(Action action, IAssemblyLoaderInitializationScope scope)
    {
        if (m_InitializationIsDone && m_Actions != null)
        {
            // Another AssemblyLoaderInitializer caused an initialization.
            // Flush the current instance, as there is no need to be in a
            // deferred execution mode anymore.
            Flush(scope);
            // Execute the action immediately.
            action();
            return;
        }

        lock (this)
        {
            var actions = m_Actions;
            if (actions != null)
            {
                // Defer the action.
                actions.Add(new() { Delegate = action, Scope = scope });
                return;
            }
        }

        // Otherwise, execute the action immediately.
        action();
    }

    /// <summary>
    /// Performs a complete flush of all initialization scopes while preserving
    /// the relative order of pending actions.
    /// </summary>
    public void Flush() => Flush(null);

    void Flush(IAssemblyLoaderInitializationScope? scope)
    {
        List<ActionDescriptor>? actions;
        bool complete, ordered;
        HashSet<IAssemblyLoaderInitializationScope> scopes;

        lock (this)
        {
            WaitForFlushCompletionCore(scope);

            actions = m_Actions;
            if (actions == null)
            {
                // Already flushed or disposed.
                return;
            }

            if (scope != null)
            {
                var newActions = actions.Where(x => x.Scope == scope).ToList();
                int n = newActions.Count;
                if (n == actions.Count)
                {
                    m_Actions = null;
                    complete = true;
                }
                else if (n != 0)
                {
                    DiscardCore(scope);
                    complete = false;
                }
                else
                {
                    complete = false;
                }
                actions = newActions;
                ordered = false;
            }
            else
            {
                m_Actions = null;
                complete = true;
                ordered = true;
            }

            // Set the value here while in lock to give it more chances to
            // get propagated quicker to other CPU cores.
            m_InitializationIsDone = true;

            // Note the scopes that are currently in progress.
            scopes = [.. actions.Select(x => x.Scope)];
            m_ScopesWithFlushInProgress.UnionWith(scopes);
        }

        List<Exception>? exceptions = null;

        try
        {
            if (complete)
                Unsubscribe();

            void ExecuteActions(IEnumerable<ActionDescriptor> actions)
            {
                foreach (var action in actions)
                {
                    try
                    {
                        action.Delegate();
                    }
                    catch (Exception e)
                    {
                        (exceptions ??= []).Add(e);
                        break;
                    }
                }
            }

            if (ordered)
            {
                // Execute pending actions in order.
                ExecuteActions(actions);
            }
            else
            {
                // Execute pending actions grouped by scopes.
                foreach (var group in actions.GroupBy(x => x.Scope))
                {
                    try
                    {
                        ExecuteActions(group);
                    }
                    finally
                    {
                        var currentScope = group.Key;
                        bool delisted;

                        lock (this)
                        {
                            delisted = m_ScopesWithFlushInProgress.Remove(currentScope);
                            Debug.Assert(delisted);

                            if (delisted)
                            {
                                // Notify about the changes.
                                Monitor.PulseAll(this);
                            }
                        }

                        if (delisted)
                        {
                            // Make a safety watchdog happy.
                            scopes.Remove(currentScope);
                        }
                    }
                }
            }
        }
        finally
        {
            if (scopes.Count != 0)
            {
                if (!ordered)
                    Debug.Fail("Safety watchdog is triggered.");

                lock (this)
                {
                    // Delist the non-delisted scopes.
                    m_ScopesWithFlushInProgress.ExceptWith(scopes);
                    // Notify about the changes.
                    Monitor.PulseAll(this);
                }
            }
        }

        if (exceptions != null)
        {
            if (exceptions.Count == 1)
                throw exceptions[0];
            else
                throw new AggregateException(exceptions);
        }
    }

    /// <summary>
    /// Signifies whether an assembly loader initialization was performed at
    /// least once.
    /// </summary>
    /// <remarks>
    /// Represented by a static field because the issues lazy initialization
    /// tries to workaround have a scope of a .NET app domain.
    /// </remarks>
    static bool m_InitializationIsDone;

    void Discard(IAssemblyLoaderInitializationScope scope)
    {
        lock (this)
        {
            DiscardCore(scope);
            WaitForFlushCompletionCore(scope);
        }
    }

    void DiscardCore(IAssemblyLoaderInitializationScope scope)
    {
        m_Actions?.RemoveAll(x => x.Scope == scope);
    }

    public void Dispose()
    {
        Unsubscribe();

        lock (this)
        {
            m_Actions = null;
            WaitForFlushCompletionCore(null);
        }
    }

    void WaitForFlushCompletionCore(IAssemblyLoaderInitializationScope? scope)
    {
        if (scope == null)
        {
            while (m_ScopesWithFlushInProgress.Count != 0)
                Monitor.Wait(this);
        }
        else
        {
            while (m_ScopesWithFlushInProgress.Contains(scope))
                Monitor.Wait(this);
        }
    }

    readonly HashSet<IAssemblyLoaderInitializationScope> m_ScopesWithFlushInProgress = [];

    List<ActionDescriptor>? m_Actions;

    readonly struct ActionDescriptor
    {
        public required Action Delegate { get; init; }
        public required IAssemblyLoaderInitializationScope Scope { get; init; }
    }

    void Unsubscribe()
    {
        var assemblyLoadPal = m_AssemblyLoadPal;
        if (assemblyLoadPal != null)
        {
            m_AssemblyLoadPal = null;
            assemblyLoadPal.Resolving -= AssemblyLoadPal_Resolving;
        }
    }

    AssemblyLoadPal? m_AssemblyLoadPal;

    Assembly? AssemblyLoadPal_Resolving(AssemblyLoadPal sender, AssemblyLoadPal.ResolvingEventArgs args)
    {
        Flush();
        return null;
    }
}
