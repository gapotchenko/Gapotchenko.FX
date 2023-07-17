// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a reentrant synchronization primitive
/// that provides a mechanism that synchronizes access to objects.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncRecursiveMonitor : AsyncMonitorImpl<AsyncRecursiveMutex>, IAsyncRecursiveMonitor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRecursiveMonitor"/> class.
    /// </summary>
    public AsyncRecursiveMonitor() :
        base(new AsyncRecursiveMutex())
    {
    }

    /// <inheritdoc/>
    public bool IsLockHeld => Lockable.IsLockHeld;
}
