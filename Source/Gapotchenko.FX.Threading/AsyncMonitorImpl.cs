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
public abstract class AsyncMonitorImpl<TMutex> : IAsyncMonitor
    where TMutex : IAsyncLockable
{
    private protected AsyncMonitorImpl(TMutex mutex, AsyncConditionVariableImpl conditionVariable)
    {
        Mutex = mutex;
        m_ConditionVariable = conditionVariable;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private protected readonly TMutex Mutex;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncConditionVariableImpl m_ConditionVariable;

    #region IAsyncLockable

    /// <inheritdoc/>
    public void Lock(CancellationToken cancellationToken = default) => Mutex.Lock(cancellationToken);

    /// <inheritdoc/>
    public Task LockAsync(CancellationToken cancellationToken = default) => Mutex.LockAsync(cancellationToken);

    /// <inheritdoc/>
    public bool TryLock() => Mutex.TryLock();

    /// <inheritdoc/>
    public bool TryLock(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        Mutex.TryLock(timeout, cancellationToken);

    /// <inheritdoc/>
    public bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        Mutex.TryLock(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        Mutex.TryLockAsync(timeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        Mutex.TryLockAsync(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public void Unlock() => Mutex.Unlock();

    /// <inheritdoc/>
    public bool IsLocked => Mutex.IsLocked;

    bool IAsyncLockable.IsRecursive => Mutex.IsRecursive;

    #endregion

    #region IConditionVariable

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void Wait() => Wait(CancellationToken.None);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void Wait(CancellationToken cancellationToken) => m_ConditionVariable.Wait(Mutex, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.Wait(Mutex, millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.Wait(Mutex, timeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task WaitAsync() => WaitAsync(CancellationToken.None);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task WaitAsync(CancellationToken cancellationToken) => m_ConditionVariable.WaitAsync(Mutex, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.WaitAsync(Mutex, millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.WaitAsync(Mutex, timeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void Notify() => m_ConditionVariable.Notify(Mutex);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void NotifyAll() => m_ConditionVariable.NotifyAll(Mutex);

    #endregion
}
