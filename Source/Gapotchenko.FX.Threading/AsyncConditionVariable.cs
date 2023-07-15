// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a synchronization primitive that is used in conjunction with an <see cref="IAsyncLockable"/>
/// to block one or more threads until another thread modifies a shared variable (the condition)
/// and notifies the <see cref="AsyncConditionVariable"/> about the change.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public class AsyncConditionVariable : IAsyncConditionVariable
{
    // ----------------------------------------------------------------------
    // Public Facade
    // ----------------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncConditionVariable"/> class
    /// with the specified <see cref="IAsyncLockable"/> instance.
    /// </summary>
    /// <param name="lockable">The <see cref="IAsyncLockable"/> instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="lockable"/> is <see langword="null"/>.</exception>
    public AsyncConditionVariable(IAsyncLockable lockable)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(lockable);

        m_Lockable = lockable;
    }

    readonly IAsyncLockable m_Lockable;
    readonly AsyncWaitQueue<bool> m_Queue = new();

    /// <summary>
    /// Gets the <see cref="IAsyncLockable"/> instance this <see cref="AsyncConditionVariable"/> is associated with.
    /// </summary>
    public IAsyncLockable Lockable => m_Lockable;

    /// <inheritdoc/>
    public void Notify()
    {
        lock (m_Queue.SyncRoot)
            m_Queue.TryDeque(true);
    }

    /// <inheritdoc/>
    public void NotifyAll()
    {
        lock (m_Queue.SyncRoot)
            m_Queue.DequeAll(true);
    }

    /// <inheritdoc/>
    public void Wait()
    {
        Wait(CancellationToken.None);
    }

    /// <inheritdoc/>
    public void Wait(CancellationToken cancellationToken)
    {
        ValidateWait();
        DoWait(cancellationToken);
    }

    /// <inheritdoc/>
    public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        ValidateWait();
        return DoWait(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    /// <inheritdoc/>
    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        ValidateWait();
        return DoWait(timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public Task WaitAsync()
    {
        return WaitAsync(CancellationToken.None);
    }

    /// <inheritdoc/>
    public Task WaitAsync(CancellationToken cancellationToken)
    {
        ValidateWait();
        return DoWaitAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        ValidateWait();
        return DoWaitAsync(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    /// <inheritdoc/>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        ValidateWait();
        return DoWaitAsync(timeout, cancellationToken);
    }

    // ----------------------------------------------------------------------
    // Core Implementation
    // ----------------------------------------------------------------------

    void DoWait(CancellationToken cancellationToken)
    {
        m_Lockable.Unlock();
        try
        {
            TaskBridge.Execute(GetAwaiter, cancellationToken);
        }
        finally
        {
            m_Lockable.Lock();
        }
    }

    bool DoWait(TimeSpan timeout, CancellationToken cancellationToken)
    {
        Debug.Assert(ExceptionHelper.IsValidTimeout(timeout));

        cancellationToken.ThrowIfCancellationRequested();

        if (timeout == TimeSpan.Zero)
        {
            return false;
        }
        else
        {
            m_Lockable.Unlock();
            try
            {
                return TaskBridge.Execute(
                    ct => TaskHelper.ExecuteWithTimeout(GetAwaiter, timeout, false, ct),
                    cancellationToken);
            }
            finally
            {
                m_Lockable.Lock();
            }
        }
    }

    Task<bool> DoWaitAsync(CancellationToken cancellationToken)
    {
        var task = GetAwaiter(cancellationToken);

        async Task<bool> ExecuteAsync()
        {
            m_Lockable.Unlock();
            try
            {
                return await task.ConfigureAwait(false);
            }
            finally
            {
                await m_Lockable.LockAsync().ConfigureAwait(false);
            }
        }

        return ExecuteAsync();
    }

    Task<bool> DoWaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        Debug.Assert(ExceptionHelper.IsValidTimeout(timeout));

        if (timeout == TimeSpan.Zero)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<bool>(cancellationToken);

            return Task.FromResult(false);
        }
        else
        {
            return TaskHelper.ExecuteWithTimeout(DoWaitAsync, timeout, false, cancellationToken);
        }
    }

    Task<bool> GetAwaiter(CancellationToken cancellationToken)
    {
        lock (m_Queue.SyncRoot)
            return m_Queue.Enqueue(cancellationToken);
    }

    void ValidateWait()
    {
        ValidateLockIsHeld();
    }

    void ValidateLockIsHeld()
    {
        if (m_Lockable is IAsyncRecursiveLockable recursiveLockable)
        {
            if (!recursiveLockable.IsLockHeld)
                throw new SynchronizationLockException();
        }
        else if (!m_Lockable.IsLocked)
        {
            throw new SynchronizationLockException();
        }
    }
}
