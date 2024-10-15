// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a synchronization primitive that, when signaled, allows one or more tasks or threads waiting on it to proceed.
/// <see cref="AsyncManualResetEvent"/> must be reset manually.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
[DebuggerDisplay("IsSet = {IsSet}")]
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
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncManualResetEvent"/> class
    /// with a Boolean value indicating whether to set the initial state to signaled.
    /// </summary>
    /// <param name="initialState">
    /// <see langword="true"/> to set the initial state to signaled;
    /// <see langword="false"/> to set the initial state to non-signaled.
    /// </param>
    public AsyncManualResetEvent(bool initialState)
    {
        if (initialState)
            m_TaskCompletionSource.SetResult(initialState);
    }

    /// <inheritdoc/>
    public void Set()
    {
        TaskCompletionSource.TrySetResult(true);
    }

    /// <inheritdoc/>
    public void Reset()
    {
        if (IsSet)
            ResetCore();
    }

    /// <inheritdoc/>
    public bool IsSet => DoWaitAsyncDirect().IsCompleted;

    /// <inheritdoc/>
    public void Wait()
    {
        DoWaitAsyncDirect().GetAwaiter().GetResult();
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
        return DoWaitAsyncDirect();
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

    bool IResetEvent.IsAutoReset => false;

    // ----------------------------------------------------------------------
    // Core Implementation
    // ----------------------------------------------------------------------

    void ResetCore()
    {
        lock (m_SyncRoot)
        {
            if (m_TaskCompletionSource.Task.IsCompleted)
                m_TaskCompletionSource = CreateTaskCompletionSource();
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly object m_SyncRoot = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TaskCompletionSource<bool> TaskCompletionSource => Volatile.Read(ref m_TaskCompletionSource);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TaskCompletionSource<bool> m_TaskCompletionSource = CreateTaskCompletionSource();

    static TaskCompletionSource<bool> CreateTaskCompletionSource() => new(TaskCreationOptions.RunContinuationsAsynchronously);

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

        if (timeout == TimeSpan.Zero) // use a quick path when possible
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<bool>(cancellationToken);

            var task = DoWaitAsyncDirect();
            if (task.IsCompleted)
                return task;
            else
                return Task.FromResult(false);
        }
        else
        {
            return TaskHelper.ExecuteWithTimeout(DoWaitAsync, timeout, false, cancellationToken);
        }
    }

    Task<bool> DoWaitAsync(CancellationToken cancellationToken) => DoWaitAsyncDirect().WaitAsync(cancellationToken);

    Task<bool> DoWaitAsyncDirect() => TaskCompletionSource.Task;
}
