// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;

namespace Gapotchenko.FX.Threading;

readonly partial struct AsyncWaitQueue<T>
{
    public AsyncWaitQueue()
    {
    }

    readonly Deque<TaskCompletionSource<T>> m_Queue = new();

    public bool IsEmpty => m_Queue.Count == 0;

    /// <summary>
    /// Enqueues a task that will be completed when it is dequeued.
    /// </summary>
    /// <param name="state">The state to use as the underlying <see cref="Task.AsyncState"/>.</param>
    /// <returns>A task that will be completed when it is dequeued.</returns>
    public Task<T> Enqueue(object? state)
    {
        var tcs = new TaskCompletionSource<T>(state, TaskCreationOptions.RunContinuationsAsynchronously);
        m_Queue.PushBack(tcs);
        return tcs.Task;
    }

    /// <summary>
    /// Tries to dequeue and complete a task with the specified result value.
    /// </summary>
    /// <param name="result">The result value to complete a dequeued task with.</param>
    /// <returns>
    /// <see langword="true"/> if a task was dequeued and completed;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryDeque(T result)
    {
        while (m_Queue.TryPopFront(out var tcs))
        {
            if (tcs.TrySetResult(result))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Dequeues and completes all tasks with the specified result value.
    /// </summary>
    /// <param name="result">The result value to complete dequeued tasks with.</param>
    /// <returns>The number of dequeued and completed tasks.</returns>
    public int DequeAll(T result)
    {
        int count = 0;
        foreach (var tcs in m_Queue)
        {
            if (tcs.TrySetResult(result))
                ++count;
        }
        m_Queue.Clear();
        return count;
    }

    /// <summary>
    /// Tries to remove and cancel the specified task using the specified cancellation token.
    /// </summary>
    /// <param name="task">The task to cancel.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the task with.</param>
    /// <returns>
    /// <see langword="true"/> if the task was removed and canceled;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryCancel(Task task, CancellationToken cancellationToken)
    {
        var queue = m_Queue;
        int count = queue.Count;

        for (int i = 0; i < count; ++i)
        {
            var tcs = queue[i];
            if (tcs.Task == task)
            {
                queue.RemoveAt(i);
                return tcs.TrySetCanceled(cancellationToken);
            }
        }

        return false;
    }

    /// <summary>
    /// Removes and cancels all tasks using the specified cancellation token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel removed tasks with.</param>
    /// <returns>The number of removed and canceled tasks.</returns>
    public int CancelAll(CancellationToken cancellationToken)
    {
        int count = 0;
        foreach (var tcs in m_Queue)
        {
            if (tcs.TrySetCanceled(cancellationToken))
                ++count;
        }
        m_Queue.Clear();
        return count;
    }

    /// <summary>
    /// Gets the synchronization object
    /// that can be used for thread-safe access to the <see cref="AsyncWaitQueue{T}"/>.
    /// </summary>
    public object SyncRoot => m_Queue;
}
