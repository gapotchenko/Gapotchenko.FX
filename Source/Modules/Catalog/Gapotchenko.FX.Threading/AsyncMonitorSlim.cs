// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a non-reentrant synchronization primitive
/// that provides a mechanism that synchronizes access to objects.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
#if PREVIEW
public
#endif
sealed class AsyncMonitorSlim : AsyncMonitorImpl<IAsyncLockable>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncMonitorSlim"/> class.
    /// </summary>
    public AsyncMonitorSlim() :
        base(new AsyncCriticalSection(), new AsyncConditionVariableImpl())
    {
    }

    internal AsyncMonitorSlim(IAsyncLockable mutex, AsyncConditionVariableImpl conditionVariable) :
        base(mutex, conditionVariable)
    {
        Debug.Assert(!mutex.IsRecursive);
    }

    /// <summary>
    /// Gets an <see cref="AsyncMonitorSlim"/> associated with the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>An <see cref="AsyncMonitorSlim"/> associated with the <paramref name="obj"/>.</returns>
    public static AsyncMonitorSlim For(object obj)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(obj);

        return AsyncMonitorObjectTable.GetDescriptor(obj).SlimMonitor;
    }
}
