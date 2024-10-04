// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of an event that supports both synchronous and asynchronous operations.
/// Event is a synchronization primitive that, when signaled, allows one or more tasks waiting on it to proceed.
/// </summary>
public interface IAsyncEvent : IEvent, IAsyncAwaitable
{
}
