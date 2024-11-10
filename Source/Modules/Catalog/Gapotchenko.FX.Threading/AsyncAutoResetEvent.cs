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
/// <see cref="AsyncAutoResetEvent"/> resets automatically after releasing a single waiting task or thread.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
[DebuggerDisplay("IsSet = {IsSet}")]
public sealed class AsyncAutoResetEvent : IAsyncResetEvent
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

    /// <inheritdoc/>
    public void Wait()
    {
        DoWait(Timeout.InfiniteTimeSpan, CancellationToken.None);
    }

    /// <inheritdoc/>
    public void Wait(CancellationToken cancellationToken)
    {
        DoWait(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <inheritdoc/>
    public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        return DoWait(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    /// <inheritdoc/>
    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return DoWait(timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public Task WaitAsync()
    {
        return DoWaitAsync(Timeout.InfiniteTimeSpan, CancellationToken.None);
    }

    /// <inheritdoc/>
    public Task WaitAsync(CancellationToken cancellationToken)
    {
        return DoWaitAsync(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        return DoWaitAsync(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    /// <inheritdoc/>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return DoWaitAsync(timeout, cancellationToken);
    }

    bool IResetEvent.IsAutoReset => true;

    // ----------------------------------------------------------------------
    // Core Implementation
    // ----------------------------------------------------------------------

    bool DoWait(TimeSpan timeout, CancellationToken cancellationToken)
    {
        Debug.Assert(ExceptionHelper.IsValidTimeout(timeout));

        return TaskBridge.Execute(
            ct => DoWaitAsync(timeout, ct),
            cancellationToken);
    }

    Task<bool> DoWaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
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
            else if (timeout == TimeSpan.Zero) // use a quick path when possible
            {
                if (cancellationToken.IsCancellationRequested)
                    return Task.FromCanceled<bool>(cancellationToken);

                return Task.FromResult(false);
            }
            else
            {
                return TaskHelper.ExecuteWithTimeout(m_Queue.Enqueue, timeout, false, cancellationToken);
            }
        }
    }
}
