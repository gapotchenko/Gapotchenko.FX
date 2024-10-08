// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a reentrant synchronization primitive
/// that provides a mechanism that synchronizes access to objects.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncMonitor : AsyncMonitorImpl<IAsyncRecursiveLockable>, IAsyncRecursiveMonitor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncMonitor"/> class.
    /// </summary>
    public AsyncMonitor() :
        base(new AsyncLock(), new AsyncConditionVariableImpl())
    {
    }

    internal AsyncMonitor(IAsyncRecursiveLockable mutex, AsyncConditionVariableImpl conditionVariable) :
        base(mutex, conditionVariable)
    {
        Debug.Assert(mutex.IsRecursive);
    }

    /// <inheritdoc/>
    public bool IsLockedByCurrentThread => Mutex.IsLockedByCurrentThread;

    /// <summary>
    /// Gets an <see cref="AsyncMonitor"/> associated with the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>An <see cref="AsyncMonitor"/> associated with the <paramref name="obj"/>.</returns>
    public static AsyncMonitor For(object obj)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(obj);

        return AsyncMonitorObjectTable.GetDescriptor(obj).Monitor;
    }
}
