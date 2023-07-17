// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a non-reentrant synchronization primitive
/// that provides a mechanism that synchronizes access to objects.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncMonitor : AsyncMonitorImpl<AsyncMutex>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncMonitor"/> class.
    /// </summary>
    public AsyncMonitor() :
        base(new AsyncMutex())
    {
    }
}
