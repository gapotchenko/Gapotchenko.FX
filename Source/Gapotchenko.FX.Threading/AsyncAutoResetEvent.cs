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

    /// <inheritdoc/>
    public void WaitOne(CancellationToken cancellationToken = default)
    {
        DoWaitOne(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <inheritdoc/>
    public bool WaitOne(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        return DoWaitOne(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    /// <inheritdoc/>
    public bool WaitOne(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return DoWaitOne(timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public Task WaitOneAsync(CancellationToken cancellationToken = default)
    {
        return DoWaitOneAsync(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<bool> WaitOneAsync(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        return DoWaitOneAsync(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    /// <inheritdoc/>
    public Task<bool> WaitOneAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return DoWaitOneAsync(timeout, cancellationToken);
    }

    bool IAsyncEvent.IsAutoReset => true;

    // ----------------------------------------------------------------------

    bool DoWaitOne(TimeSpan timeout, CancellationToken cancellationToken) =>
        TaskBridge.Execute(DoWaitOneAsync(timeout, cancellationToken));

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
}
