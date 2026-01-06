// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a reentrant concurrency primitive
/// that provides a mechanism that synchronizes access to objects.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncMonitor :
    AsyncMonitorImpl<IAsyncReentrableLockable>,
    IAsyncRecursiveMonitor,
    IAsyncReentrableLockable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncMonitor"/> class.
    /// </summary>
    public AsyncMonitor() :
        base(new AsyncLock(), new AsyncConditionVariableImpl())
    {
    }

    internal AsyncMonitor(IAsyncReentrableLockable mutex, AsyncConditionVariableImpl conditionVariable) :
        base(mutex, conditionVariable)
    {
        Debug.Assert(mutex.IsRecursive);
    }

    void IReentrableLockable.Enter(int recursionLevel, CancellationToken cancellationToken) =>
        Mutex.Enter(recursionLevel, cancellationToken);

    Task IAsyncReentrableLockable.EnterAsync(int recursionLevel, CancellationToken cancellationToken) =>
        Mutex.EnterAsync(recursionLevel, cancellationToken);

    int IReentrableLockable.ExitAll() => Mutex.ExitAll();

    /// <inheritdoc/>
    public bool IsLockedByCurrentThread => Mutex.IsLockedByCurrentThread;

    /// <summary>
    /// Gets an <see cref="IAsyncRecursiveMonitor"/> associated with the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>An <see cref="IAsyncRecursiveMonitor"/> associated with the <paramref name="obj"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public static IAsyncRecursiveMonitor For(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return AsyncMonitorObjectTable.GetDescriptor(obj).Monitor;
    }
}
