// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Threading;

/// <summary>
/// The interface of a recursive lockable synchronization primitive
/// that can be completely exited and then reentered.
/// The primitive supports synchronous operations.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IReentrableLockable : IRecursiveLockable
{
    /// <summary>
    /// Enters the lock the specified number of times, that is, recursively,
    /// waiting if necessary until the lock can be entered.
    /// </summary>
    /// <inheritdoc cref="ILockable.Enter(CancellationToken)"/>
    /// <param name="cancellationToken"><inheritdoc/></param>
    /// <param name="recursionLevel">The number of times to enter the lock recursively.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="recursionLevel"/> is negative.</exception>
    void Enter(int recursionLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exits the lock until it is not held anymore.
    /// </summary>
    /// <remarks>
    /// If the current task holds the lock multiple times, such as recursively, the lock is exited until it is no longer held.
    /// </remarks>
    /// <returns>
    /// The number of times the lock was exited,
    /// or <c>0</c> if the current task does not hold the lock.
    /// </returns>
    int ExitAll();
}
