// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a synchronization primitive that, when signaled, allows one or more threads waiting on it to proceed.
/// <see cref="AsyncAutoResetEvent"/> resets automatically after releasing a single waiting thread.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncAutoResetEvent : IAsyncEvent
{
    // ----------------------------------------------------------------------
    // Public Facade
    // ----------------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncAutoResetEvent"/> class
    /// with a Boolean value indicating whether to set the initial state to signaled.
    /// </summary>
    /// <param name="initialState">
    /// <see langword="true"/> to set the initial state to signaled;
    /// <see langword="false"/> to set the initial state to non-signaled.
    /// </param>
    public AsyncAutoResetEvent(bool initialState)
    {
        m_Set = initialState;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool m_Set;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncWaitQueue<bool> m_Queue = new();

    /// <inheritdoc/>
    public void Set()
    {
        lock (m_Queue.SyncRoot)
        {
            if (!m_Queue.TryDeque(true))
                m_Set = true;
        }
    }

    /// <inheritdoc/>
    public void Reset()
    {
        Volatile.Write(ref m_Set, false);
    }

    /// <inheritdoc/>
    public bool IsSet => Volatile.Read(ref m_Set);

    /// <inheritdoc cref="IAsyncEvent.Wait(CancellationToken)"/>
    public void WaitOne(CancellationToken cancellationToken = default)
    {
        DoWaitOne(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <inheritdoc cref="IAsyncEvent.Wait(int, CancellationToken)"/>
    public bool WaitOne(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        return DoWaitOne(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    /// <inheritdoc cref="IAsyncEvent.Wait(TimeSpan, CancellationToken)"/>
    public bool WaitOne(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return DoWaitOne(timeout, cancellationToken);
    }

    /// <inheritdoc cref="IAsyncEvent.WaitAsync(CancellationToken)"/>
    public Task WaitOneAsync(CancellationToken cancellationToken = default)
    {
        return DoWaitOneAsync(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <inheritdoc cref="IAsyncEvent.WaitAsync(int, CancellationToken)"/>
    public Task<bool> WaitOneAsync(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        return DoWaitOneAsync(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    /// <inheritdoc cref="IAsyncEvent.WaitAsync(TimeSpan, CancellationToken)"/>
    public Task<bool> WaitOneAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return DoWaitOneAsync(timeout, cancellationToken);
    }

    bool IAsyncEvent.IsAutoReset => true;

    // ----------------------------------------------------------------------
    // Core Implementation
    // ----------------------------------------------------------------------

    bool DoWaitOne(TimeSpan timeout, CancellationToken cancellationToken) =>
        TaskBridge.Execute(
            ct => DoWaitOneAsync(timeout, ct),
            cancellationToken);

    Task<bool> DoWaitOneAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        Debug.Assert(ExceptionHelper.IsValidTimeout(timeout));

        lock (m_Queue.SyncRoot)
        {
            if (m_Set)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Task.FromCanceled<bool>(cancellationToken);

                m_Set = false;
                return Task.FromResult(true);
            }
            else
            {
                return m_Queue.Enqueue(timeout, false, cancellationToken);
            }
        }
    }

    #region Compatibility

    void IAsyncEvent.Wait(CancellationToken cancellationToken) => WaitOne(cancellationToken);

    bool IAsyncEvent.Wait(int millisecondsTimeout, CancellationToken cancellationToken) => WaitOne(millisecondsTimeout, cancellationToken);

    bool IAsyncEvent.Wait(TimeSpan timeout, CancellationToken cancellationToken) => WaitOne(timeout, cancellationToken);

    Task IAsyncEvent.WaitAsync(CancellationToken cancellationToken) => WaitOneAsync(cancellationToken);

    Task<bool> IAsyncEvent.WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken) => WaitOneAsync(millisecondsTimeout, cancellationToken);

    Task<bool> IAsyncEvent.WaitAsync(TimeSpan timeout, CancellationToken cancellationToken) => WaitOneAsync(timeout, cancellationToken);

    #endregion
}
