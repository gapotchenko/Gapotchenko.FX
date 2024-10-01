namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a synchronization primitive that can be awaited,
/// and supports both synchronous and asynchronous operations.
/// </summary>
public interface IAsyncAwaitable
{
    /// <summary>
    /// Blocks the current thread until the synchronization primitive receives a signal.
    /// </summary>
    void Wait();

    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    /// <inheritdoc cref="Wait()"/>
    void Wait(CancellationToken cancellationToken);

    /// <summary>
    /// Blocks the current thread until the synchronization primitive receives a signal,
    /// using a 32-bit signed integer that specifies the timeout in milliseconds.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the synchronization primitive receives a signal,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="int.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Blocks the current thread until the synchronization primitive receives a signal,
    /// using a <see cref="TimeSpan"/> to measure the time interval.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the synchronization primitive receives a signal,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously waits until the synchronization primitive receives a signal.
    /// </summary>
    /// <inheritdoc cref="Wait()"/>
    Task WaitAsync();

    /// <summary>
    /// Asynchronously waits until the synchronization primitive receives a signal.
    /// </summary>
    /// <inheritdoc cref="Wait(CancellationToken)"/>
    Task WaitAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously waits until the synchronization primitive receives a signal,
    /// using a 32-bit signed integer that specifies the timeout in milliseconds.
    /// </summary>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the synchronization primitive receives a signal,
    /// otherwise with a result of <see langword="false"/>.
    /// </returns>
    /// <inheritdoc cref="Wait(int, CancellationToken)"/>
    Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously waits until the synchronization primitive receives a signal,
    /// using a 32-bit signed integer that specifies the timeout in milliseconds.
    /// </summary>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the synchronization primitive receives a signal,
    /// otherwise with a result of <see langword="false"/>.
    /// </returns>
    /// <inheritdoc cref="Wait(TimeSpan, CancellationToken)"/>
    Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
}
