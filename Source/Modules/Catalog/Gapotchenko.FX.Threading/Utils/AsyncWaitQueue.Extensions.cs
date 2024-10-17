// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Utils;

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

        if (cancellationToken.CanBeCanceled)
        {
            var queue = this;

            void Cancel()
            {
                lock (queue.SyncRoot)
                    queue.TryComplete(task, tcs => tcs.TrySetCanceled(cancellationToken));
            }

            var ctr = cancellationToken.Register(Cancel);
            _ = task.Finally(ctr.Dispose);
        }

        return task;
    }
}
