﻿using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a reentrant synchronization primitive
/// that ensures that only one thread can access a resource at any given time.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncRecursiveMutex : IAsyncMutex
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AsyncMutexCoreImpl m_CoreImpl = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AsyncRecursionTracker m_RecursionTracker = new();

    /// <inheritdoc/>
    public void Lock(CancellationToken cancellationToken = default)
    {
        if (m_RecursionTracker.IsFirstLevel)
            m_CoreImpl.Lock(cancellationToken);
        m_RecursionTracker.Enter();
    }

    /// <inheritdoc/>
    public Task LockAsync(CancellationToken cancellationToken = default)
    {
        if (m_RecursionTracker.IsFirstLevel)
        {
            var task = m_CoreImpl.LockAsync(cancellationToken);

            // Suppress the flow of the execution context to be able to propagate a possible rollback.
            ExecutionContext.SuppressFlow();

            return task.ContinueWith(
                task =>
                {
                    if (ExecutionContext.IsFlowSuppressed())
                        ExecutionContext.RestoreFlow();

                    // Rethrow the exception if any.
                    task.GetAwaiter().GetResult();

                    m_RecursionTracker.Enter();
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
                TaskScheduler.Default);
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
        if (m_RecursionTracker.IsFirstLevel)
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
    public Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        TryLockAsyncCore(() => m_CoreImpl.TryLockAsync(timeout, cancellationToken));

    /// <inheritdoc/>
    public Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        TryLockAsyncCore(() => m_CoreImpl.TryLockAsync(millisecondsTimeout, cancellationToken));

    Task<bool> TryLockAsyncCore(Func<Task<bool>> func)
    {
        if (m_RecursionTracker.IsFirstLevel)
        {
            var task = func();

            // Suppress the flow of the execution context to be able to propagate a possible rollback.
            ExecutionContext.SuppressFlow();

            return task.ContinueWith(
                task =>
                {
                    if (ExecutionContext.IsFlowSuppressed())
                        ExecutionContext.RestoreFlow();

                    bool locked = task.GetAwaiter().GetResult();
                    if (locked)
                        m_RecursionTracker.Enter();

                    return locked;
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
                TaskScheduler.Default);
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

    bool IAsyncLockable.IsRecursive => true;
}
