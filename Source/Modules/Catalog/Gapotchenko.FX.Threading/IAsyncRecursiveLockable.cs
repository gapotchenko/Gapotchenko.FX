// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// The interface of a recursive lockable synchronization primitive
/// that supports both synchronous and asynchronous operations.
/// </summary>
public interface IAsyncRecursiveLockable : IAsyncLockable
{
    /// <summary>
    /// Gets a value indicating whether the lock is held by the current task.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current task holds the lock; otherwise, <see langword="false"/>.
    /// </value>
    bool IsLockedByCurrentTask { get; }
}
