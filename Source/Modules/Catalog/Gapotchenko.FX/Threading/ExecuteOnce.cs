// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides a thread-safe strategy which delays the execution of an action until it is explicitly asserted with <see cref="EnsureExecuted"/> method.
/// </summary>
[DebuggerDisplay("IsExecuted={IsExecuted}")]
#if TFF_HOST_PROTECTION
[HostProtection(Synchronization = true, ExternalThreading = true)]
#endif
public struct ExecuteOnce
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecuteOnce"/> structure.
    /// </summary>
    /// <param name="action">The action.</param>
    public ExecuteOnce(Action action) :
        this(action, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecuteOnce"/> structure.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="syncLock">
    /// An object used as the mutually exclusive lock for action execution.
    /// When the given value is <see langword="null"/>, an unique synchronization lock object is used.
    /// </param>
    public ExecuteOnce(Action action, object? syncLock)
    {
        ArgumentNullException.ThrowIfNull(action);

        m_Action = action;
        m_SyncLock = syncLock;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecuteOnce"/> structure.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="syncLock">
    /// A <see cref="Lock"/> object used as the mutually exclusive lock for action execution.
    /// When the given value is <see langword="null"/>, an unique synchronization lock object is used.
    /// </param>
    public ExecuteOnce(Action action, Lock? syncLock) :
#pragma warning disable CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement.
        this(action, (object?)syncLock)
#pragma warning restore CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement.
    {
    }

    /// <summary>
    /// Ensures that the action was executed.
    /// </summary>
    public void EnsureExecuted()
    {
        if (m_SyncLock is Lock)
            LazyInitializerEx.EnsureInitialized(ref Unsafe.As<object?, Lock?>(ref m_SyncLock), ref m_Action);
        else
            LazyInitializerEx.EnsureInitialized(ref m_SyncLock, ref m_Action);
    }

    /// <summary>
    /// Gets a value indicating whether the action was executed.
    /// </summary>
    public bool IsExecuted =>
        Volatile.Read(ref m_Action) is null &&
        m_SyncLock is not null; // a check for m_SyncLock is needed to cover the uninitialized struct scenario

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object? m_SyncLock;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Action? m_Action;
}
