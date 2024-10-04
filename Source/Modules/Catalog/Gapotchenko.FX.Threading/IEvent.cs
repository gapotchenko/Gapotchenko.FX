// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of an event that supports synchronous operations.
/// Event is a synchronization primitive that, when signaled, allows one or more threads waiting on it to proceed.
/// </summary>
public interface IEvent : IAwaitable
{
    /// <summary>
    /// Gets a value indicating whether the event is set.
    /// </summary>
    bool IsSet { get; }
}
