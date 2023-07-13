// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;
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
    AsyncMutexImpl m_CoreImpl = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AsyncRecursionTracker m_RecursionTracker = new();

    /// <inheritdoc/>
    public void Lock(CancellationToken cancellationToken = default)
    {
        if (!m_RecursionTracker.IsEntered)
            m_CoreImpl.Lock(cancellationToken);
        m_RecursionTracker.Enter();
    }

    /// <inheritdoc/>
    public Task LockAsync(CancellationToken cancellationToken = default)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocal(m_RecursionTracker.Enter);

            async Task ExecuteAsync()
            {
                using (asyncLocalChanges)
                {
                    await m_CoreImpl.LockAsync(cancellationToken).ConfigureAwait(false);
                    asyncLocalChanges.Commit();
                }
            }

            return ExecuteAsync();
        }
        else
        {
            // Already locked by the current thread.
            m_RecursionTracker.Enter();
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc/>
    public bool TryLock() => TryLock(0);

    /// <inheritdoc/>
    public bool TryLock(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        TryLockCore(() => m_CoreImpl.TryLock(timeout, cancellationToken));

    /// <inheritdoc/>
    public bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        TryLockCore(() => m_CoreImpl.TryLock(millisecondsTimeout, cancellationToken));

    bool TryLockCore(Func<bool> func)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            bool locked = func();
            if (locked)
                m_RecursionTracker.Enter();
            return locked;
        }
        else
        {
            // Already locked by the current thread.
            m_RecursionTracker.Enter();
            return true;
        }
    }

    /// <inheritdoc/>
    public Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return TryLockAsyncCore(() => m_CoreImpl.TryLockAsync(timeout, cancellationToken));
    }

    /// <inheritdoc/>
    public Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        return TryLockAsyncCore(() => m_CoreImpl.TryLockAsync(millisecondsTimeout, cancellationToken));
    }

    Task<bool> TryLockAsyncCore(Func<Task<bool>> func)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocal(m_RecursionTracker.Enter);

            async Task<bool> ExecuteAsync()
            {
                using (asyncLocalChanges)
                {
                    bool locked = await func().ConfigureAwait(false);
                    if (locked)
                        asyncLocalChanges.Commit();
                    return locked;
                }
            }

            return ExecuteAsync();
        }
        else
        {
            // Already locked by the current thread.
            m_RecursionTracker.Enter();
            return Task.FromResult(true);
        }
    }

    /// <inheritdoc/>
    public void Unlock()
    {
        if (m_RecursionTracker.Leave())
            m_CoreImpl.Unlock();
    }

    /// <inheritdoc/>
    public bool IsLocked => m_CoreImpl.IsLocked;

    /// <inheritdoc/>
    public bool IsLockHeld => m_RecursionTracker.IsEntered;

    bool IAsyncLockable.IsRecursive => true;
}
