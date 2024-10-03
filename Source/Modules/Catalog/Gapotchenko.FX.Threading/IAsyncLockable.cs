﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// The interface of a lockable synchronization primitive
/// that supports both synchronous and asynchronous operations.
/// </summary>
public interface IAsyncLockable
{
    /// <summary>
    /// Enters the lock, waiting if necessary until the lock can be entered.
    /// </summary>
    /// <remarks>
    /// When the method returns, the current task is the only task that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered.
    /// </remarks>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    void Enter(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously enters the lock, waiting if necessary until the lock can be entered.
    /// </summary>
    /// <remarks><inheritdoc cref="Enter(CancellationToken)"/></remarks>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete when the lock is entered.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    Task EnterAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to enter the lock without waiting.
    /// </summary>
    /// <remarks>
    /// When the method returns <see langword="true"/>, the current task is the only task that holds the lock.
    /// If the lock can't be entered immediately, the method returns <see langword="false"/> without waiting for the lock.
    /// </remarks>
    /// <returns>
    /// <see langword="true"/> if the lock was entered by the current task; otherwise, <see langword="false"/>.
    /// </returns>
    bool TryEnter();

    /// <summary>
    /// Tries to enter the lock, waiting if necessary until the lock can be entered or until the specified timeout expires.
    /// </summary>
    /// <remarks><inheritdoc cref="TryEnter()"/></remarks>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait until the lock can be entered.
    /// Specify a value that represents <see cref="Timeout.Infinite"/> (<c>-1</c>) milliseconds to wait indefinitely,
    /// or a value that represents <c>0</c> milliseconds to not wait.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the lock was entered by the current task; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/>, after its conversion to an integer millisecond value, represents a value that is less
    /// than <c>-1</c> milliseconds or greater than <see cref="int.MaxValue"/> milliseconds.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    bool TryEnter(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to enter the lock, waiting if necessary for the specified number of milliseconds until the lock can be entered.
    /// </summary>
    /// <remarks><inheritdoc cref="TryEnter(TimeSpan, CancellationToken)"/></remarks>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the lock was entered by the current task; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is less than <c>-1</c>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    bool TryEnter(int millisecondsTimeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously tries to enter the lock, waiting if necessary until the lock can be entered or until the specified timeout expires.
    /// </summary>
    /// <remarks><inheritdoc cref="TryEnter(TimeSpan, CancellationToken)"/></remarks>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the lock was entered by the current task;
    /// otherwise, the returned task will complete with a result of <see langword="false"/>.
    /// </returns>
    /// <inheritdoc cref="TryEnter(TimeSpan, CancellationToken)"/>
    Task<bool> TryEnterAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously tries to enter the lock, waiting if necessary for the specified number of milliseconds until the lock can be entered.
    /// </summary>
    /// <remarks><inheritdoc cref="TryEnter(int, CancellationToken)"/></remarks>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the lock was entered by the current task;
    /// otherwise, the returned task will complete with a result of <see langword="false"/>.
    /// </returns>
    /// <inheritdoc cref="TryEnter(int, CancellationToken)"/>
    Task<bool> TryEnterAsync(int millisecondsTimeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exits the lock.
    /// </summary>
    /// <remarks>
    /// If the current task holds the lock multiple times, such as recursively, the lock is exited only once.
    /// The current task should ensure that each enter is matched with an exit.
    /// </remarks>
    /// <exception cref="SynchronizationLockException">
    /// The current task does not hold the lock.
    /// </exception>
    void Exit();

    /// <summary>
    /// Gets a value indicating whether a task holds a lock on the synchronization primitive.
    /// </summary>
    bool IsEntered { get; }

    /// <summary>
    /// Gets a value indicating whether the synchronization primitive supports locking recursion.
    /// </summary>
    bool IsRecursive { get; }
}
