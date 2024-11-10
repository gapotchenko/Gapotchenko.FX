// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;

namespace Gapotchenko.FX.Threading.Utils;

readonly partial struct AsyncWaitQueue<T>()
{
    /// <summary>
    /// Enqueues a task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>.
    /// </summary>
    /// <param name="state">The state to use as the underlying <see cref="Task.AsyncState"/>.</param>
    /// <returns>A task that will be completed when it is dequeued from the <see cref="AsyncWaitQueue{T}"/>.</returns>
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
    /// Dequeues all tasks from the <see cref="AsyncWaitQueue{T}"/> and completes them with the specified result value.
    /// </summary>
    /// <param name="result">The result value to complete dequeued tasks with.</param>
    /// <returns>The number of dequeued and completed tasks.</returns>
    public int DequeAll(T result) => CompleteAll(tcs => tcs.TrySetResult(result));

    public bool IsEmpty => m_Queue.Count == 0;

    /// <summary>
    /// Tries to remove and complete the specified task using the specified completion function.
    /// </summary>
    /// <param name="task">The task to remove and complete.</param>
    /// <param name="completionFunc">The task completion function.</param>
    /// <returns>
    /// <see langword="true"/> if the task was removed and completed;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryComplete(Task task, Func<TaskCompletionSource<T>, bool> completionFunc)
    {
        var queue = m_Queue;
        int count = queue.Count;

        for (int i = 0; i < count; ++i)
        {
            var tcs = queue[i];
            if (tcs.Task == task)
            {
                // It's important to change the task state *before* removing it from the queue
                // in order to avoid logical integrity violations when this method is thread-aborted.
                // Otherwise, the task theoretically can become removed but not completed,
                // causing a deadlock on the waiters' side as nobody would be able to ever change the state
                // of the already removed task.
                var completed = completionFunc(tcs);
                queue.RemoveAt(i);
                return completed;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes all tasks from the <see cref="AsyncWaitQueue{T}"/> and completes them using the specified completion function.
    /// </summary>
    /// <param name="completionFunc">The task completion function.</param>
    /// <returns>The number of removed and completed tasks.</returns>
    public int CompleteAll(Func<TaskCompletionSource<T>, bool> completionFunc)
    {
        int count = 0;
        foreach (var tcs in m_Queue)
        {
            if (completionFunc(tcs))
                ++count;
        }
        m_Queue.Clear();
        return count;
    }

    /// <summary>
    /// Gets the synchronization object
    /// that can be used for a thread-safe access to the <see cref="AsyncWaitQueue{T}"/>.
    /// </summary>
    public object SyncRoot => m_Queue;

    readonly Deque<TaskCompletionSource<T>> m_Queue = new();
}
