// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a synchronization primitive that can be awaited,
/// and supports both synchronous and asynchronous operations.
/// </summary>
public interface IAsyncAwaitable : IAwaitable
{
    /// <summary>
    /// Asynchronously waits until the synchronization primitive receives a signal.
    /// </summary>
    /// <inheritdoc cref="IAwaitable.Wait()"/>
    Task WaitAsync();

    /// <summary>
    /// Asynchronously waits until the synchronization primitive receives a signal.
    /// </summary>
    /// <inheritdoc cref="IAwaitable.Wait(CancellationToken)"/>
    Task WaitAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously waits until the synchronization primitive receives a signal,
    /// using a 32-bit signed integer that specifies the timeout in milliseconds.
    /// </summary>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the synchronization primitive receives a signal;
    /// otherwise, the task will complete with a result of <see langword="false"/>.
    /// </returns>
    /// <inheritdoc cref="IAwaitable.Wait(int, CancellationToken)"/>
    Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously waits until the synchronization primitive receives a signal,
    /// using a <see cref="TimeSpan"/> to measure the time interval.
    /// </summary>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the synchronization primitive receives a signal;
    /// otherwise, the task will complete with a result of <see langword="false"/>.
    /// </returns>
    /// <inheritdoc cref="IAwaitable.Wait(TimeSpan, CancellationToken)"/>
    Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
}
