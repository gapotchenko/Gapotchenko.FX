// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a reentrant synchronization primitive
/// that ensures that only one task or thread can access a resource at any given time.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncLock : IAsyncRecursiveMutex, IAsyncReentrableLockable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLock"/> class.
    /// </summary>
    public AsyncLock()
    {
    }

    /// <inheritdoc/>
    public void Enter(CancellationToken cancellationToken = default) => m_CoreImpl.Enter(cancellationToken);

    void IReentrableLockable.Enter(int recursionLevel, CancellationToken cancellationToken) => m_CoreImpl.Enter(recursionLevel, cancellationToken);

    /// <inheritdoc/>
    public Task EnterAsync(CancellationToken cancellationToken = default) => m_CoreImpl.EnterAsync(cancellationToken);

    Task IAsyncReentrableLockable.EnterAsync(int recursionLevel, CancellationToken cancellationToken) => m_CoreImpl.EnterAsync(recursionLevel, cancellationToken);

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

    int IReentrableLockable.ExitAll() => m_CoreImpl.ExitAll();

    /// <inheritdoc/>
    public bool IsEntered => m_CoreImpl.IsEntered;

    /// <inheritdoc/>
    public bool IsLockedByCurrentThread => m_CoreImpl.IsLockedByCurrentThread;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncRecursiveLockableImpl<AsyncMutexImpl> m_CoreImpl = new(new());

    bool ILockable.IsRecursive => true;
}
