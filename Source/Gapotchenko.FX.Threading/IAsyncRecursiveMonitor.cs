// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a reentrant synchronization primitive that provides a mechanism that synchronizes access to objects.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public interface IAsyncRecursiveMonitor : IAsyncMonitor, IAsyncRecursiveLockable
{
}
