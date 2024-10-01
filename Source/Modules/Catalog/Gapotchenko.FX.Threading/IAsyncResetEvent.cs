// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a resettable event that supports both synchronous and asynchronous operations.
/// Event is a synchronization primitive that, when signaled, allows one or more threads waiting on it to proceed.
/// </summary>
public interface IAsyncResetEvent : IAsyncEvent
{
    /// <summary>
    /// Sets the state of the event to signaled,
    /// allowing one or more threads waiting on the event to proceed.
    /// </summary>
    void Set();

    /// <summary>
    /// Sets the state of the event to non-signaled,
    /// causing threads to block.
    /// </summary>
    void Reset();

    /// <summary>
    /// Gets a value indicating whether the event is an auto reset event as opposed to a manual reset event.
    /// </summary>
    bool IsAutoReset { get; }
}
