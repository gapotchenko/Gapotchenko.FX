// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a synchronization primitive that is used in conjunction with an <see cref="IAsyncLockable"/>
/// to block one or more threads until another thread modifies a shared variable or state (the condition)
/// and notifies the <see cref="AsyncConditionVariable"/> about the change.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public class AsyncConditionVariable : IAsyncConditionVariable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncConditionVariable"/> class
    /// with the specified <see cref="IAsyncLockable"/> instance.
    /// </summary>
    /// <param name="lockable">The <see cref="IAsyncLockable"/> instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="lockable"/> is <see langword="null"/>.</exception>
    public AsyncConditionVariable(IAsyncLockable lockable)
    {
        ArgumentNullException.ThrowIfNull(lockable);

        m_Lockable = lockable;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly IAsyncLockable m_Lockable;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncConditionVariableImpl m_CoreImpl = new();

    /// <summary>
    /// Gets the associated <see cref="IAsyncLockable"/> instance.
    /// </summary>
    public IAsyncLockable Lockable => m_Lockable;

    /// <inheritdoc/>
    public void Notify() => m_CoreImpl.Notify(m_Lockable);

    /// <inheritdoc/>
    public void NotifyAll() => m_CoreImpl.NotifyAll(m_Lockable);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void Wait() => Wait(CancellationToken.None);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public void Wait(CancellationToken cancellationToken) => m_CoreImpl.Wait(m_Lockable, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.Wait(m_Lockable, millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.Wait(m_Lockable, timeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task WaitAsync() => WaitAsync(CancellationToken.None);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task WaitAsync(CancellationToken cancellationToken) => m_CoreImpl.WaitAsync(m_Lockable, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.WaitAsync(m_Lockable, millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_CoreImpl.WaitAsync(m_Lockable, timeout, cancellationToken);
}
