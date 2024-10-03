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
    /// <inheritdoc/>
    public void Enter(CancellationToken cancellationToken = default) => m_CoreImpl.Enter(cancellationToken);

    /// <inheritdoc/>
    public Task EnterAsync(CancellationToken cancellationToken = default) => m_CoreImpl.EnterAsync(cancellationToken);

    /// <inheritdoc/>
    public bool TryEnter() => m_CoreImpl.TryEnter();

    /// <inheritdoc/>
    public bool TryEnter(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryEnter(timeout, cancellationToken);

    /// <inheritdoc/>
    public bool TryEnter(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryEnter(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryEnterAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryEnterAsync(timeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryEnterAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryEnterAsync(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public void Exit() => m_CoreImpl.Exit();

    /// <inheritdoc/>
    public bool IsEntered => m_CoreImpl.IsEntered;

    /// <inheritdoc/>
    public bool LockIsHeldByCurrentTask => m_CoreImpl.LockIsHeldByCurrentTask;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncRecursiveLockableImpl<AsyncMutexImpl> m_CoreImpl = new(new());

    bool IAsyncLockable.IsRecursive => true;
}
