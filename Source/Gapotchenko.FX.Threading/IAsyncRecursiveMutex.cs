// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a recursive mutex that supports both synchronous and asynchronous operations.
/// Mutex is a synchronization primitive that ensures that only one thread can access a resource at any given time.
/// </summary>
public interface IAsyncRecursiveMutex : IAsyncMutex, IAsyncRecursiveLockable
{
}
