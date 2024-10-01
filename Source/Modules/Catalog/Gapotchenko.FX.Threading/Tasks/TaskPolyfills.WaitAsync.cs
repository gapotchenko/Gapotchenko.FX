// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if NET6_0_OR_GREATER
#define TFF_TASK_WAITASYNC
#endif

using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Tasks;

#pragma warning disable CA1062

partial class TaskPolyfills
{
    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when this <see cref="Task"/> completes
    /// or when the specified timeout expires.
    /// </summary>
    /// <param name="task">The <see cref="Task"/> to wait for.</param>
    /// <param name="timeout">
    /// The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/>
    /// if it has not otherwise completed.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> representing the asynchronous wait.
    /// It may or may not be the same instance as the current instance.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0,
    /// or a positive integer less than or equal to the maximum allowed timer duration.
    /// </exception>
#if TFF_TASK_WAITASYNC
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Task WaitAsync(
#if !TFF_TASK_WAITASYNC
        this
#endif
        Task task, TimeSpan timeout)
    {
#if TFF_TASK_WAITASYNC
        return task.WaitAsync(timeout);
#else
        ExceptionHelper.ThrowIfArgumentIsNull(task);
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return WaitAsyncCore(task, timeout, CancellationToken.None);
#endif
    }

    /// <summary>
    /// Gets a <see cref="Task{TResult}"/> that will complete when this <see cref="Task{TResult}"/> completes
    /// or when the specified timeout expires.
    /// </summary>
    /// <param name="task">The <see cref="Task{TResult}"/> to wait for.</param>
    /// <param name="timeout">
    /// The timeout after which the <see cref="Task{TResult}"/> should be faulted with a <see cref="TimeoutException"/>
    /// if it has not otherwise completed.
    /// </param>
    /// <returns>
    /// The <see cref="Task{TResult}"/> representing the asynchronous wait.
    /// It may or may not be the same instance as the current instance.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0,
    /// or a positive integer less than or equal to the maximum allowed timer duration.
    /// </exception>
#if TFF_TASK_WAITASYNC
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Task<TResult> WaitAsync<TResult>(
#if !TFF_TASK_WAITASYNC
        this
#endif
        Task<TResult> task, TimeSpan timeout)
    {
#if TFF_TASK_WAITASYNC
        return task.WaitAsync(timeout);
#else
        ExceptionHelper.ThrowIfArgumentIsNull(task);
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return WaitAsyncCore(task, timeout, CancellationToken.None);
#endif
    }

    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when this <see cref="Task"/> completes
    /// or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// </summary>
    /// <param name="task">The <see cref="Task"/> to wait for.</param>
    /// <param name="cancellationToken">
    /// The <see cref="CancellationToken"/> to monitor for a cancellation request.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> representing the asynchronous wait.
    /// It may or may not be the same instance as the current instance.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
#if TFF_TASK_WAITASYNC
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Task WaitAsync(
#if !TFF_TASK_WAITASYNC
        this
#endif
        Task task, CancellationToken cancellationToken)
    {
#if TFF_TASK_WAITASYNC
        return task.WaitAsync(cancellationToken);
#else
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        return WaitAsyncCore(task, Timeout.InfiniteTimeSpan, cancellationToken);
#endif
    }

    /// <summary>
    /// Gets a <see cref="Task{TResult}"/> that will complete when this <see cref="Task{TResult}"/> completes
    /// or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// </summary>
    /// <param name="task">The <see cref="Task{TResult}"/> to wait for.</param>
    /// <param name="cancellationToken">
    /// The <see cref="CancellationToken"/> to monitor for a cancellation request.
    /// </param>
    /// <returns>
    /// The <see cref="Task{TResult}"/> representing the asynchronous wait.
    /// It may or may not be the same instance as the current instance.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
#if TFF_TASK_WAITASYNC
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Task<TResult> WaitAsync<TResult>(
#if !TFF_TASK_WAITASYNC
        this
#endif
        Task<TResult> task, CancellationToken cancellationToken)
    {
#if TFF_TASK_WAITASYNC
        return task.WaitAsync(cancellationToken);
#else
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        return WaitAsyncCore(task, Timeout.InfiniteTimeSpan, cancellationToken);
#endif
    }

    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when this <see cref="Task"/> completes,
    /// when the specified timeout expires,
    /// or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// </summary>
    /// <param name="task">The <see cref="Task"/> to wait for.</param>
    /// <param name="timeout">
    /// The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/>
    /// if it has not otherwise completed.
    /// </param>
    /// <param name="cancellationToken">
    /// The <see cref="CancellationToken"/> to monitor for a cancellation request.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> representing the asynchronous wait.
    /// It may or may not be the same instance as the current instance.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0,
    /// or a positive integer less than or equal to the maximum allowed timer duration.
    /// </exception>
    /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
#if TFF_TASK_WAITASYNC
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Task WaitAsync(
#if !TFF_TASK_WAITASYNC
        this
#endif
        Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
#if TFF_TASK_WAITASYNC
        return task.WaitAsync(timeout, cancellationToken);
#else
        ExceptionHelper.ThrowIfArgumentIsNull(task);
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return WaitAsyncCore(task, timeout, cancellationToken);
#endif
    }

    /// <summary>
    /// Gets a <see cref="Task{TResult}"/> that will complete when this <see cref="Task{TResult}"/> completes,
    /// when the specified timeout expires,
    /// or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// </summary>
    /// <param name="task">The <see cref="Task{TResult}"/> to wait for.</param>
    /// <param name="timeout">
    /// The timeout after which the <see cref="Task{TResult}"/> should be faulted with a <see cref="TimeoutException"/>
    /// if it has not otherwise completed.
    /// </param>
    /// <param name="cancellationToken">
    /// The <see cref="CancellationToken"/> to monitor for a cancellation request.
    /// </param>
    /// <returns>
    /// The <see cref="Task{TResult}"/> representing the asynchronous wait.
    /// It may or may not be the same instance as the current instance.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0,
    /// or a positive integer less than or equal to the maximum allowed timer duration.
    /// </exception>
    /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
#if TFF_TASK_WAITASYNC
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Task<TResult> WaitAsync<TResult>(
#if !TFF_TASK_WAITASYNC
        this
#endif
        Task<TResult> task, TimeSpan timeout, CancellationToken cancellationToken)
    {
#if TFF_TASK_WAITASYNC
        return task.WaitAsync(timeout, cancellationToken);
#else
        ExceptionHelper.ThrowIfArgumentIsNull(task);
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        return WaitAsyncCore(task, timeout, cancellationToken);
#endif
    }

    // ----------------------------------------------------------------------

#if !TFF_TASK_WAITASYNC

    static Task WaitAsyncCore(Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        if (task.IsCompleted)
            return task;

        bool canBeCanceled = cancellationToken.CanBeCanceled;
        bool canBeTimedOut = timeout != Timeout.InfiniteTimeSpan;

        if (!canBeCanceled)
        {
            if (!canBeTimedOut)
                return task;
        }
        else if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        if (timeout == TimeSpan.Zero)
            return Task.FromException(new TimeoutException());

        Debug.Assert(canBeTimedOut || canBeCanceled);

        return ExecuteAsync();

        async Task ExecuteAsync()
        {
            using var cts = CancellationTokenSourceHelper.CreateLinked(cancellationToken);

            var controlTask = Task.Delay(timeout, cts.Token);

            Task? completedTask = null;
            try
            {
                completedTask = await Task.WhenAny(task, controlTask).ConfigureAwait(false);
            }
            finally
            {
                if (completedTask != controlTask)
                {
                    // Cancel the control task so that it does not linger around any longer.
                    cts.Cancel();
                }
            }

            if (completedTask == controlTask && controlTask.IsCompletedSuccessfully())
            {
                // The control task has completed due to an expired timeout.
                Debug.Assert(canBeTimedOut);
                throw new TimeoutException();
            }

            await completedTask.ConfigureAwait(false);
        }
    }

    static Task<TResult> WaitAsyncCore<TResult>(Task<TResult> task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        if (task.IsCompleted)
            return task;

        bool canBeCanceled = cancellationToken.CanBeCanceled;
        bool canBeTimedOut = timeout != Timeout.InfiniteTimeSpan;

        if (!canBeCanceled)
        {
            if (!canBeTimedOut)
                return task;
        }
        else if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<TResult>(cancellationToken);
        }

        if (timeout == TimeSpan.Zero)
            return Task.FromException<TResult>(new TimeoutException());

        Debug.Assert(canBeTimedOut || canBeCanceled);

        return ExecuteAsync();

        async Task<TResult> ExecuteAsync()
        {
            using var cts = CancellationTokenSourceHelper.CreateLinked(cancellationToken);

            var ct = cts.Token;
            var controlTask =
                Task.Delay(timeout, ct)
                // Adapt the control task result to TResult to make a compiler happy.
                .ContinueWith(
                    _ => default(TResult)!, // this value will be discarded, it's only needed for the type system integrity
                    ct,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Default);

            Task<TResult>? completedTask = null;
            try
            {
                completedTask = await Task.WhenAny(task, controlTask).ConfigureAwait(false);
            }
            finally
            {
                if (completedTask != controlTask)
                {
                    // Cancel the control task so that it does not linger around any longer.
                    cts.Cancel();
                }
            }

            if (completedTask == controlTask && controlTask.IsCompletedSuccessfully())
            {
                // The control task has completed due to an expired timeout.
                Debug.Assert(canBeTimedOut);
                throw new TimeoutException();
            }

            return await completedTask.ConfigureAwait(false);
        }
    }

#endif
}
