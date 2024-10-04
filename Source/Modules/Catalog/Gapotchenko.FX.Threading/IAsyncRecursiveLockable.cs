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
public interface IAsyncRecursiveLockable : IRecursiveLockable, IAsyncLockable
{
}
