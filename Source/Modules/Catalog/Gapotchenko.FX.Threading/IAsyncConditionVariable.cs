// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a synchronization primitive that is used in conjunction with an <see cref="IAsyncLockable"/>
/// to block one or more tasks until another task modifies a shared variable or state (the condition)
/// and notifies the <see cref="IAsyncConditionVariable"/> about the change.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public interface IAsyncConditionVariable : IConditionVariable, IAsyncAwaitable
{
}
