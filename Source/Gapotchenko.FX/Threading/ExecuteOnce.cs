using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides a thread-safe strategy which delays the execution of an action until its explicitly asserted with <see cref="EnsureExecuted"/> method.
/// </summary>
[DebuggerDisplay("IsExecuted={IsExecuted}")]
#if TFF_HOST_PROTECTION
[HostProtection(Synchronization = true, ExternalThreading = true)]
#endif
public struct ExecuteOnce
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecuteOnce"/> struct.
    /// </summary>
    /// <param name="action">The action.</param>
    public ExecuteOnce(Action action) :
        this(action, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecuteOnce"/> struct.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="syncLock">
    /// An object used as the mutually exclusive lock for action execution.
    /// When the given value is null, an unique synchronization lock object is used.
    /// </param>
    public ExecuteOnce(Action action, object? syncLock)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        m_Action = action;
        m_SyncLock = syncLock;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object? m_SyncLock;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Action? m_Action;

    /// <summary>
    /// Ensures that the action was executed.
    /// </summary>
    public void EnsureExecuted() => LazyInitializerEx.EnsureInitialized(ref m_SyncLock, ref m_Action);

    /// <summary>
    /// Gets a value indicating whether the action was executed.
    /// </summary>
    public bool IsExecuted => Volatile.Read(ref m_Action) == null && m_SyncLock != null; // check for _SyncLock is needed to cover uninitialized struct scenario
}
