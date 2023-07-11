// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;

namespace Gapotchenko.FX.Threading;

partial struct AsyncWaitQueue<T>
{
    /// <summary>
    /// Enqueues a task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>.
    /// </summary>
    /// <returns>A task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>.</returns>
    public Task<T> Enqueue() => Enqueue(null);

    /// <summary>
    /// Enqueues a cancelable task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>.</returns>
    public Task<T> Enqueue(CancellationToken cancellationToken) => Enqueue(null, cancellationToken);

    /// <summary>
    /// Enqueues a cancelable task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="state">The state to use as the underlying <see cref="Task.AsyncState"/>.</param>
    /// <returns>A task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>.</returns>
    public Task<T> Enqueue(object? state, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<T>(cancellationToken);

        var task = Enqueue(state);
        if (!cancellationToken.CanBeCanceled)
            return task;

        var queue = this;

        void Cancel()
        {
            lock (queue.SyncRoot)
                queue.TryCancel(task, cancellationToken);
        }

        TaskHelper.ContinueWithDispose(task, cancellationToken.Register(Cancel));

        return task;
    }

    /// <summary>
    /// Enqueues a cancelable task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>,
    /// or will return the specified result if the timeout expires before the task is dequeued.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <param name="timeoutResult">The result to return on timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>,
    /// or will return <paramref name="timeoutResult"/> if the timeout expires before the task is dequeued.
    /// </returns>
    public Task<T> Enqueue(TimeSpan timeout, T timeoutResult, CancellationToken cancellationToken) =>
        Enqueue(null, timeout, timeoutResult, cancellationToken);

    /// <summary>
    /// Enqueues a cancelable task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>,
    /// or will return the specified result if the timeout expires before the task is dequeued.
    /// </summary>
    /// <param name="state">The state to use as the underlying <see cref="Task.AsyncState"/>.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="timeoutResult">The result to return on timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>,
    /// or will return <paramref name="timeoutResult"/> if the timeout expires before the task is dequeued.
    /// </returns>
    public Task<T> Enqueue(object? state, TimeSpan timeout, T timeoutResult, CancellationToken cancellationToken)
    {
        var queue = this;

        return TaskHelper.ExecuteWithTimeoutAsync(
            ct => queue.Enqueue(state, ct),
            timeout,
            timeoutResult,
            cancellationToken);
    }
}
