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
    /// Gets a value indicating whether the current thread holds a lock on this synchronization primitive.
    /// </summary>
    bool IsLockHeld { get; }
}
