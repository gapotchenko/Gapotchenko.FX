using System.Diagnostics;

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
        if (m_RecursionTracker.Enter())
        {
            try
            {
                m_CoreImpl.Lock(cancellationToken);
            }
            catch
            {
                // Rollback.
                m_RecursionTracker.Leave();
                throw;
            }
        }
    }

    /// <inheritdoc/>
    public Task LockAsync(CancellationToken cancellationToken = default)
    {
        if (m_RecursionTracker.Enter())
        {
            Task task;
            try
            {
                task = m_CoreImpl.LockAsync(cancellationToken);
            }
            catch
            {
                // Rollback.
                m_RecursionTracker.Leave();
                throw;
            }

            // Suppress the flow of the execution context to be able to propagate a possible rollback.
            var flowControl = ExecutionContext.SuppressFlow();
            var parentThreadId = Environment.CurrentManagedThreadId;

            return task.ContinueWith(
                task =>
                {
                    if (Environment.CurrentManagedThreadId == parentThreadId)
                        flowControl.Undo();

                    if (task.Status is TaskStatus.Faulted or TaskStatus.Canceled)
                    {
                        // Rollback.
                        m_RecursionTracker.Leave();
                        // Rethrow the exception.
                        task.GetAwaiter().GetResult();
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }
        else
        {
            // Already locked by the current thread.
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
        if (m_RecursionTracker.Enter())
        {
            bool locked;
            try
            {
                locked = func();
            }
            catch
            {
                // Rollback.
                m_RecursionTracker.Leave();
                throw;
            }
            if (!locked)
            {
                // Rollback.
                m_RecursionTracker.Leave();
            }
            return locked;
        }
        else
        {
            // Already locked by the current thread.
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
        if (m_RecursionTracker.Enter())
        {
            Task<bool> task;

            try
            {
                task = func();
            }
            catch
            {
                // Rollback.
                m_RecursionTracker.Leave();
                throw;
            }

            // Suppress the flow of the execution context to be able to propagate a possible rollback.
            var flowControl = ExecutionContext.SuppressFlow();
            var parentThreadId = Environment.CurrentManagedThreadId;

            return task.ContinueWith(
                task =>
                {
                    if (Environment.CurrentManagedThreadId == parentThreadId)
                        flowControl.Undo();

                    if (task.Status is TaskStatus.Faulted or TaskStatus.Canceled)
                    {
                        // Rollback.
                        m_RecursionTracker.Leave();
                        // Rethrow the exception.
                        task.GetAwaiter().GetResult();
                    }

                    bool locked = task.Result;
                    if (!locked)
                    {
                        // Rollback.
                        m_RecursionTracker.Leave();
                    }

                    return locked;
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }
        else
        {
            // Already locked by the current thread.
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
