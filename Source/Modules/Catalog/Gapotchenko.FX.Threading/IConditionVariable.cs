// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a synchronization primitive that is used in conjunction with an <see cref="ILockable"/>
/// to block one or more threads until another thread modifies a shared variable or state (the condition)
/// and notifies the <see cref="IConditionVariable"/> about the change.
/// The primitive supports synchronous operations.
/// </summary>
public interface IConditionVariable : IAwaitable
{
    /// <summary>
    /// Notifies a thread in the waiting queue of a change in the locked shared state.
    /// </summary>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    void Notify();

    /// <summary>
    /// Notifies all waiting threads of a change in the locked shared state.
    /// </summary>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    void NotifyAll();
}
