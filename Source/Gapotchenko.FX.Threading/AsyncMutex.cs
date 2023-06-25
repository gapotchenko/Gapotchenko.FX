using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a non-reentrant synchronization primitive
/// that ensures that only one thread can access a resource at any given time.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncMutex
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AsyncMutexCoreImpl m_CoreImpl = new();

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public void Lock(CancellationToken cancellationToken = default) => m_CoreImpl.Lock(cancellationToken);

    /// <summary>
    /// Tries to lock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully locked the <see cref="AsyncMutex"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryLock() => TryLock(0);

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncMutex"/>,
    /// using a <see cref="TimeSpan"/> that specifies the timeout.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the <see cref="AsyncMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully locked the <see cref="AsyncMutex"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public bool TryLock(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryLock(timeout, cancellationToken);

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncMutex"/>,
    /// using a 32-bit signed integer that specifies the timeout.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the <see cref="AsyncMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully locked the <see cref="AsyncMutex"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.TryLock(millisecondsTimeout, cancellationToken);

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete when the <see cref="AsyncMutex"/> has been locked.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Task LockAsync(CancellationToken cancellationToken = default) => m_CoreImpl.LockAsync(cancellationToken);

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncMutex"/>,
    /// using a <see cref="TimeSpan"/> to measure the time interval.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the <see cref="AsyncMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the current thread successfully locked the <see cref="AsyncMutex"/>,
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
        m_CoreImpl.TryLockAsync(timeout, cancellationToken);

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncMutex"/>,
    /// using a 32-bit signed integer to measure the time interval.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the <see cref="AsyncMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the current thread successfully locked the <see cref="AsyncMutex"/>,
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
        m_CoreImpl.TryLockAsync(millisecondsTimeout, cancellationToken);

    /// <summary>
    /// Unlocks the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot unlock a non-locked <see cref="AsyncMutex"/>.</exception>
    public void Unlock() => m_CoreImpl.Unlock();

    /// <summary>
    /// Gets a value indicating whether the <see cref="AsyncMutex"/> is locked by a thread.
    /// </summary>
    public bool IsLocked => m_CoreImpl.IsLocked;

    /// <summary>
    /// Defines a disposable scope of an <see cref="AsyncMutex"/> access to a resource.
    /// The scope can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// </summary>
    public readonly struct Scope : IDisposable
    {
        internal Scope(AsyncMutex? mutex)
        {
            m_Mutex = mutex;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly AsyncMutex? m_Mutex;

        /// <summary>
        /// Gets a value indicating whether the <see cref="AsyncMutex"/> was locked in this scope.
        /// </summary>
        public bool WasLocked => m_Mutex != null;

        /// <summary>
        /// Unlocks the <see cref="AsyncMutex"/> if it was locked in this scope.
        /// </summary>
        public void Dispose()
        {
            m_Mutex?.Unlock();
        }
    }

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncMutex"/>,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A scope that can be disposed to unlock the <see cref="AsyncMutex"/>.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Scope LockScope(CancellationToken cancellationToken = default)
    {
        Lock(cancellationToken);
        return new Scope(this);
    }

    /// <summary>
    /// Tries to lock the <see cref="AsyncMutex"/> and
    /// returns a disposable scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <returns>
    /// A scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// <see cref="Scope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncMutex"/>.
    /// </returns>
    public Scope TryLockScope() => TryLockScope(0);

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncMutex"/>,
    /// using a <see cref="TimeSpan"/> that specifies the timeout,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the <see cref="AsyncMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// <see cref="Scope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncMutex"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Scope TryLockScope(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(TryLock(timeout, cancellationToken) ? this : null);

    /// <summary>
    /// Blocks the current thread until it can lock the <see cref="AsyncMutex"/>,
    /// using a <see cref="TimeSpan"/> that specifies the timeout,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the <see cref="AsyncMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// <see cref="Scope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncMutex"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Scope TryLockScope(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        new(TryLock(millisecondsTimeout, cancellationToken) ? this : null);

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncMutex"/>,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <returns>
    /// A task that will complete when the <see cref="AsyncMutex"/> has been locked with a result of the scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// </returns>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public async Task<Scope> LockScopeAsync(CancellationToken cancellationToken = default)
    {
        await LockAsync(cancellationToken).ConfigureAwait(false);
        return new Scope(this);
    }

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncMutex"/>,
    /// using a <see cref="TimeSpan"/> to measure the time interval,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the <see cref="AsyncMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of the scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// <see cref="Scope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncMutex"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public async Task<Scope> TryLockScopeAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(await TryLockAsync(timeout, cancellationToken).ConfigureAwait(false) ? this : null);

    /// <summary>
    /// Asynchronously waits to lock the <see cref="AsyncMutex"/>,
    /// using a 32-bit signed integer to measure the time interval,
    /// and returns a disposable scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the <see cref="AsyncMutex"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of the scope that can be disposed to unlock the <see cref="AsyncMutex"/>.
    /// <see cref="Scope.WasLocked"/> property indicates whether the current thread successfully locked the <see cref="AsyncMutex"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public async Task<Scope> TryLockScopeAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        new(await TryLockAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false) ? this : null);
}
