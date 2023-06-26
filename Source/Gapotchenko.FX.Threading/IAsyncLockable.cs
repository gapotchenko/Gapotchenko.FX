namespace Gapotchenko.FX.Threading;

/// <summary>
/// The interface of a lockable synchronization primitive that supports both synchronous and asynchronous operations.
/// </summary>
public interface IAsyncLockable
{
    /// <summary>
    /// Blocks the current thread until it can lock the synchronization primitive.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    void Lock(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously waits to lock the synchronization primitive.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete when the synchronization primitive has been locked.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    Task LockAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to lock the synchronization primitive.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully locked the synchronization primitive,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool TryLock();

    /// <summary>
    /// Blocks the current thread until it can lock the synchronization primitive,
    /// using a <see cref="TimeSpan"/> that specifies the timeout.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully locked the synchronization primitive,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    bool TryLock(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Blocks the current thread until it can lock the synchronization primitive,
    /// using a 32-bit signed integer that specifies the timeout.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully locked the synchronization primitive,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously waits to lock the synchronization primitive,
    /// using a <see cref="TimeSpan"/> to measure the time interval.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the current thread successfully locked the synchronization primitive,
    /// otherwise with a result of <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously waits to lock the synchronization primitive,
    /// using a 32-bit signed integer to measure the time interval.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the current thread successfully locked the synchronization primitive,
    /// otherwise with a result of <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlocks the synchronization primitive.
    /// </summary>
    /// <exception cref="InvalidOperationException">Unbalanced lock/unlock acquisitions of a thread synchronization primitive.</exception>
    void Unlock();
}
