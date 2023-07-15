// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a synchronization primitive that is used in conjunction with an <see cref="IAsyncLockable"/>
/// to block one or more threads until another thread modifies a shared variable (the condition)
/// and notifies the <see cref="AsyncConditionVariable"/> about the change.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public interface IAsyncConditionVariable : IAsyncAwaitable
{
    /// <summary>
    /// Notifies a thread in the waiting queue of a change in the locked shared variable.
    /// </summary>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    void Notify();

    /// <summary>
    /// Notifies all waiting threads of a change in the locked shared variable.
    /// </summary>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    void NotifyAll();
}
