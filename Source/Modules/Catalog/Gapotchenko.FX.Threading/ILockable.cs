// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Threading;

/// <summary>
/// The interface of a lockable synchronization primitive
/// that supports synchronous operations.
/// </summary>
public interface ILockable
{
    /// <summary>
    /// Enters the lock, waiting if necessary until the lock can be entered.
    /// </summary>
    /// <remarks>
    /// When the method returns, the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered.
    /// </remarks>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="LockRecursionException">
    /// The lock has reached the limit of repeated entries by the current thread.
    /// The limit is implementation-defined and is intended to be high enough that it would not be reached in normal situations.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    void Enter(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to enter the lock without waiting.
    /// </summary>
    /// <remarks>
    /// When the method returns <see langword="true"/>, the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method returns <see langword="false"/> without waiting for the lock.
    /// </remarks>
    /// <returns>
    /// <see langword="true"/> if the lock was entered by the current thread; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="LockRecursionException">
    /// The lock has reached the limit of repeated entries by the current thread.
    /// The limit is implementation-defined and is intended to be high enough that it would not be reached in normal situations.
    /// </exception>
    bool TryEnter();

    /// <summary>
    /// Tries to enter the lock, waiting if necessary until the lock can be entered or until the specified timeout expires.
    /// </summary>
    /// <remarks>
    /// When the method returns <see langword="true"/>, the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered or
    /// until the timeout specified by the <paramref name="timeout"/> parameter expires.
    /// If the timeout expires before entering the lock, the method returns <see langword="false"/>.
    /// </remarks>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait until the lock can be entered.
    /// Specify a value that represents <see cref="Timeout.InfiniteTimeSpan"/> (<c>-1</c>) milliseconds to wait indefinitely,
    /// or a value that represents <c>0</c> milliseconds to not wait.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/>, after its conversion to an integer millisecond value, represents a value that is less
    /// than <c>-1</c> milliseconds or greater than <see cref="int.MaxValue"/> milliseconds.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    /// <inheritdoc cref="TryEnter()"/>
    bool TryEnter(TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to enter the lock, waiting if necessary for the specified number of milliseconds until the lock can be entered.
    /// </summary>
    /// <remarks>
    /// When the method returns <see langword="true"/>, the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered or
    /// until the timeout specified by the <paramref name="millisecondsTimeout"/> parameter expires.
    /// If the timeout expires before entering the lock, the method returns <see langword="false"/>.
    /// </remarks>
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
    /// <exception cref="LockRecursionException">
    /// The lock has reached the limit of repeated entries by the current thread.
    /// The limit is implementation-defined and is intended to be high enough that it would not be reached in normal situations.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    bool TryEnter(int millisecondsTimeout, CancellationToken cancellationToken = default);

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
    /// Gets a value indicating whether some thread holds a lock on the synchronization primitive.
    /// </summary>
    bool IsEntered { get; }

    /// <summary>
    /// Gets a value indicating whether the synchronization primitive supports locking recursion.
    /// </summary>
    bool IsRecursive { get; }
}
