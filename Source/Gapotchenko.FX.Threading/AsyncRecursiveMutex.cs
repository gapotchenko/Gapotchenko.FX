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

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
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
                m_RecursionTracker.Leave();
                throw;
            }
        }
    }
    /// <summary>
    /// Tries to lock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryLock() => TryLock(0);

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncRecursiveMutex"/>,
    /// using a <see cref="TimeSpan"/> that specifies the timeout.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the <see cref="AsyncRecursiveMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public bool TryLock(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (m_RecursionTracker.Enter())
        {
            bool locked;
            try
            {
                locked = m_CoreImpl.TryLock(timeout, cancellationToken);
            }
            catch
            {
                m_RecursionTracker.Leave();
                throw;
            }
            if (!locked)
                m_RecursionTracker.Leave();
            return locked;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncRecursiveMutex"/>,
    /// using a 32-bit signed integer that specifies the timeout.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the <see cref="AsyncRecursiveMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        if (m_RecursionTracker.Enter())
        {
            bool locked;
            try
            {
                locked = m_CoreImpl.TryLock(millisecondsTimeout, cancellationToken);
            }
            catch
            {
                m_RecursionTracker.Leave();
                throw;
            }
            if (!locked)
                m_RecursionTracker.Leave();
            return locked;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete when the <see cref="AsyncRecursiveMutex"/> has been locked.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Task LockAsync(CancellationToken cancellationToken = default)
    {
        if (m_RecursionTracker.Enter())
            return LockAsyncCore(cancellationToken);
        else
            return Task.CompletedTask;
    }

    async Task LockAsyncCore(CancellationToken cancellationToken)
    {
        try
        {
            await m_CoreImpl.LockAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            m_RecursionTracker.Leave();
            throw;
        }
    }

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncRecursiveMutex"/>,
    /// using a <see cref="TimeSpan"/> to measure the time interval.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the <see cref="AsyncRecursiveMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>,
    /// otherwise with a result of <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        TryLockAsyncCore(() => m_CoreImpl.TryLockAsync(timeout, cancellationToken));

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncRecursiveMutex"/>,
    /// using a 32-bit signed integer to measure the time interval.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the <see cref="AsyncRecursiveMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>,
    /// otherwise with a result of <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        TryLockAsyncCore(() => m_CoreImpl.TryLockAsync(millisecondsTimeout, cancellationToken));

    Task<bool> TryLockAsyncCore(Func<Task<bool>> func)
    {
        if (m_RecursionTracker.Enter())
            return Inner(func);
        else
            return Task.FromResult(true);

        async Task<bool> Inner(Func<Task<bool>> func)
        {
            if (m_RecursionTracker.Enter())
            {
                bool locked;
                try
                {
                    locked = await func().ConfigureAwait(false);
                }
                catch
                {
                    m_RecursionTracker.Leave();
                    throw;
                }
                if (!locked)
                    m_RecursionTracker.Leave();
                return locked;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Unlocks the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <exception cref="LockRecursionException">Unbalanced lock/unlock acquisitions of a thread synchronization primitive.</exception>
    public void Unlock()
    {
        if (m_RecursionTracker.Leave())
            m_CoreImpl.Unlock();
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="AsyncRecursiveMutex"/> is locked by a thread.
    /// </summary>
    public bool IsLocked => m_CoreImpl.IsLocked;

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncRecursiveMutex"/>,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public AsyncMutexScope LockScope(CancellationToken cancellationToken = default)
    {
        Lock(cancellationToken);
        return new AsyncMutexScope(this);
    }

    /// <summary>
    /// Tries to lock the <see cref="AsyncRecursiveMutex"/> and
    /// returns a disposable scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <returns>
    /// A scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// <see cref="AsyncMutexScope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>.
    /// </returns>
    public AsyncMutexScope TryLockScope() => TryLockScope(0);

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncRecursiveMutex"/>,
    /// using a <see cref="TimeSpan"/> that specifies the timeout,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the <see cref="AsyncRecursiveMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// <see cref="AsyncMutexScope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public AsyncMutexScope TryLockScope(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(TryLock(timeout, cancellationToken) ? this : null);

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncRecursiveMutex"/>,
    /// using a <see cref="TimeSpan"/> that specifies the timeout,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the <see cref="AsyncRecursiveMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// <see cref="AsyncMutexScope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public AsyncMutexScope TryLockScope(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        new(TryLock(millisecondsTimeout, cancellationToken) ? this : null);

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncRecursiveMutex"/>,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <returns>
    /// A task that will complete when the <see cref="AsyncRecursiveMutex"/> has been locked with a result of the scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// </returns>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Task<AsyncMutexScope> LockScopeAsync(CancellationToken cancellationToken = default) =>
        LockAsync(cancellationToken)
        .ContinueWith(
            _ => new AsyncMutexScope(this),
            cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled,
            TaskScheduler.Default);

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncRecursiveMutex"/>,
    /// using a <see cref="TimeSpan"/> to measure the time interval,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the <see cref="AsyncRecursiveMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of the scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// <see cref="AsyncMutexScope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public async Task<AsyncMutexScope> TryLockScopeAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(await TryLockAsync(timeout, cancellationToken).ConfigureAwait(false) ? this : null);

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncRecursiveMutex"/>,
    /// using a 32-bit signed integer to measure the time interval,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the <see cref="AsyncRecursiveMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of the scope that can be disposed to unlock the <see cref="AsyncRecursiveMutex"/>.
    /// <see cref="AsyncMutexScope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncRecursiveMutex"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Task<AsyncMutexScope> TryLockScopeAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        TryLockAsync(millisecondsTimeout, cancellationToken)
        .ContinueWith(
            task => new AsyncMutexScope(task.Result ? this : null),
            cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted,
            TaskScheduler.Default);
}
