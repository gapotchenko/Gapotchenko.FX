// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Threading;

/// <summary>
/// The interface of a recursive lockable synchronization primitive
/// that supports synchronous operations.
/// </summary>
public interface IRecursiveLockable : ILockable
{
    /// <summary>
    /// Gets a value indicating whether the lock is held by the current thread.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current thread holds the lock; otherwise, <see langword="false"/>.
    /// </value>
    bool IsLockedByCurrentThread { get; }
}
