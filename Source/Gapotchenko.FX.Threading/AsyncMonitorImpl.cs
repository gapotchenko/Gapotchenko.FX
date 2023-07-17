// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// This is an infrastructure type that should never be used by user code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class AsyncMonitorImpl<TLockable> : IAsyncMonitor
    where TLockable : IAsyncLockable
{
    private protected AsyncMonitorImpl(TLockable lockable)
    {
        Lockable = lockable;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private protected readonly TLockable Lockable;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncConditionVariableImpl m_ConditionVariable = new();

    #region IAsyncLockable

    /// <inheritdoc/>
    public void Lock(CancellationToken cancellationToken = default) => Lockable.Lock(cancellationToken);

    /// <inheritdoc/>
    public Task LockAsync(CancellationToken cancellationToken = default) => Lockable.LockAsync(cancellationToken);

    /// <inheritdoc/>
    public bool TryLock() => Lockable.TryLock();

    /// <inheritdoc/>
    public bool TryLock(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        Lockable.TryLock(timeout, cancellationToken);

    /// <inheritdoc/>
    public bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        Lockable.TryLock(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        Lockable.TryLockAsync(timeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        Lockable.TryLockAsync(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public void Unlock() => Lockable.Unlock();

    /// <inheritdoc/>
    public bool IsLocked => Lockable.IsLocked;

    bool IAsyncLockable.IsRecursive => Lockable.IsRecursive;

    #endregion

    #region IConditionVariable

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void Wait() => Wait(CancellationToken.None);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void Wait(CancellationToken cancellationToken) => m_ConditionVariable.Wait(Lockable, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.Wait(Lockable, millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.Wait(Lockable, timeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task WaitAsync() => WaitAsync(CancellationToken.None);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task WaitAsync(CancellationToken cancellationToken) => m_ConditionVariable.WaitAsync(Lockable, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.WaitAsync(Lockable, millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.WaitAsync(Lockable, timeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void Notify() => m_ConditionVariable.Notify(Lockable);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void NotifyAll() => m_ConditionVariable.NotifyAll(Lockable);

    #endregion
}
