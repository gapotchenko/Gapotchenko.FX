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
/// <see cref="AsyncManualResetEvent"/> must be reset manually.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncManualResetEvent : IAsyncResetEvent
{
    // ----------------------------------------------------------------------
    // Public Facade
    // ----------------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncManualResetEvent"/> class
    /// with an initial state of non-signaled.
    /// </summary>
    public AsyncManualResetEvent()
    {
        m_TaskCompletionSource = CreateTaskCompletionSource();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncManualResetEvent"/> class
    /// with a Boolean value indicating whether to set the initial state to signaled.
    /// </summary>
    /// <param name="initialState">
    /// <see langword="true"/> to set the initial state to signaled;
    /// <see langword="false"/> to set the initial state to non-signaled.
    /// </param>
    public AsyncManualResetEvent(bool initialState) :
        this()
    {
        if (initialState)
            m_TaskCompletionSource.SetResult(initialState);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly object m_SyncRoot = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TaskCompletionSource<bool> m_TaskCompletionSource;

    /// <inheritdoc/>
    public void Set()
    {
        Volatile.Read(ref m_TaskCompletionSource).TrySetResult(true);
    }

    /// <inheritdoc/>
    public void Reset()
    {
        lock (m_SyncRoot)
        {
            if (m_TaskCompletionSource.Task.IsCompleted)
                m_TaskCompletionSource = CreateTaskCompletionSource();
        }
    }

    /// <inheritdoc/>
    public bool IsSet => Volatile.Read(ref m_TaskCompletionSource).Task.IsCompleted;

    /// <inheritdoc/>
    public void Wait()
    {
        DoWaitAsync().GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void Wait(CancellationToken cancellationToken)
    {
        if (cancellationToken.CanBeCanceled)
            TaskBridge.Execute(DoWaitAsync, cancellationToken);
        else
            Wait();
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
        return DoWaitAsync();
    }

    /// <inheritdoc/>
    public Task WaitAsync(CancellationToken cancellationToken)
    {
        return DoWaitAsync(cancellationToken);
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

    bool IAsyncResetEvent.IsAutoReset => false;

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

    Task<bool> DoWaitAsync()
    {
        return Volatile.Read(ref m_TaskCompletionSource).Task;
    }

    Task<bool> DoWaitAsync(CancellationToken cancellationToken)
    {
        return DoWaitAsync().WaitAsync(cancellationToken);
    }

    Task<bool> DoWaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        Debug.Assert(ExceptionHelper.IsValidTimeout(timeout));

        return TaskHelper.ExecuteWithTimeoutAsync(DoWaitAsync, timeout, false, cancellationToken);
    }

    static TaskCompletionSource<bool> CreateTaskCompletionSource() => new(TaskCreationOptions.RunContinuationsAsynchronously);
}
