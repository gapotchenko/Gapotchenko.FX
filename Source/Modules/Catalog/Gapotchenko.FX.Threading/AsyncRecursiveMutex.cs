// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a reentrant synchronization primitive
/// that ensures that only one thread can access a resource at any given time.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncRecursiveMutex : IAsyncRecursiveMutex
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncRecursiveLockableImpl<AsyncMutexImpl> m_CoreImpl = new(new());

    /// <inheritdoc/>
    public void Lock(CancellationToken cancellationToken = default) => m_CoreImpl.Lock(cancellationToken);

    /// <inheritdoc/>
    public Task LockAsync(CancellationToken cancellationToken = default) => m_CoreImpl.LockAsync(cancellationToken);

    /// <inheritdoc/>
    public bool TryLock() => m_CoreImpl.TryLock();

    /// <inheritdoc/>
    public bool TryLock(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryLock(timeout, cancellationToken);

    /// <inheritdoc/>
    public bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryLock(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryLockAsync(timeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryLockAsync(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public void Unlock() => m_CoreImpl.Unlock();

    /// <inheritdoc/>
    public bool IsLocked => m_CoreImpl.IsLocked;

    /// <inheritdoc/>
    public bool IsHeldByCurrentTask => m_CoreImpl.IsHeldByCurrentTask;

    bool IAsyncLockable.IsRecursive => true;
}
