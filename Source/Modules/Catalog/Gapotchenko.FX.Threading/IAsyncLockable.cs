// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// The interface of a lockable synchronization primitive
/// that supports both synchronous and asynchronous operations.
/// </summary>
public interface IAsyncLockable : ILockable
{
    /// <summary>
    /// Asynchronously enters the lock, waiting if necessary until the lock can be entered.
    /// </summary>
    /// <remarks>
    /// When the method returns, the current task is the only task that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered.
    /// </remarks>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete when the lock is entered.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    Task EnterAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously tries to enter the lock, waiting if necessary until the lock can be entered or until the specified timeout expires.
    /// </summary>
    /// <remarks>
    /// When the method returns <see langword="true"/>, the current task is the only task that holds the lock.
    /// If the lock can't be entered immediately, the method returns <see langword="false"/> without waiting for the lock.
    /// </remarks>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the lock was entered by the current task;
    /// otherwise, the returned task will complete with a result of <see langword="false"/>.
    /// </returns>
    /// <inheritdoc cref="ILockable.TryEnter(TimeSpan, CancellationToken)"/>
    Task<bool> TryEnterAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously tries to enter the lock, waiting if necessary for the specified number of milliseconds until the lock can be entered.
    /// </summary>
    /// <remarks><inheritdoc cref="TryEnterAsync(TimeSpan, CancellationToken)"/></remarks>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the lock was entered by the current task;
    /// otherwise, the returned task will complete with a result of <see langword="false"/>.
    /// </returns>
    /// <inheritdoc cref="ILockable.TryEnter(int, CancellationToken)"/>
    Task<bool> TryEnterAsync(int millisecondsTimeout, CancellationToken cancellationToken = default);
}
