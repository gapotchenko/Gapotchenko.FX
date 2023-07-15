// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Tasks;

/// <summary>
/// Bridges synchronous and asynchronous task execution models together.
/// </summary>
public static class TaskBridge
{
    // ----------------------------------------------------------------------
    // Public Facade
    // ----------------------------------------------------------------------

    /// <summary>
    /// Synchronously completes the execution of an already started asynchronous <see cref="Task"/>.
    /// </summary>
    /// <param name="task">The asynchronous <see cref="Task"/> to execute.</param>
    public static void Execute(Task task)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        // Use a quick path when possible.
        if (task.IsCompleted)
        {
            task.GetAwaiter().GetResult(); // rethrow the task exception, if any
            return;
        }

        ExecuteCore(() => task);
    }

#if TFF_VALUETASK
    /// <summary>
    /// Synchronously completes the execution of an already started asynchronous <see cref="ValueTask"/>.
    /// </summary>
    /// <param name="task">The asynchronous <see cref="ValueTask"/> to execute.</param>
    public static void Execute(in ValueTask task)
    {
        // Use a quick path when possible.
        if (task.IsCompleted)
        {
            task.GetAwaiter().GetResult(); // rethrow the task exception, if any
            return;
        }

        var t = task.AsTask();
        ExecuteCore(() => t);
    }
#endif

    /// <summary>
    /// Synchronously executes an asynchronous <see cref="Task"/>.
    /// </summary>
    /// <param name="task">The function that returns an asynchronous <see cref="Task"/> to execute.</param>
    public static void Execute(Func<Task> task)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        ExecuteCore(task);
    }

    /// <summary>
    /// Synchronously completes the execution of an already started asynchronous <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="task">The asynchronous <see cref="Task{TResult}"/> to execute.</param>
    /// <returns>A result of the executed task.</returns>
    public static TResult Execute<TResult>(Task<TResult> task)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        // Use a quick path when possible.
        if (task.IsCompleted)
            return task.GetAwaiter().GetResult();

        return ExecuteCore(() => task);
    }

#if TFF_VALUETASK
    /// <summary>
    /// Synchronously completes the execution of an already started asynchronous <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <param name="task">The asynchronous <see cref="ValueTask{TResult}"/> to execute.</param>
    /// <returns>A result of the executed task.</returns>
    public static TResult Execute<TResult>(in ValueTask<TResult> task)
    {
        // Use a quick path when possible.
        if (task.IsCompleted)
            return task.GetAwaiter().GetResult();

        var t = task.AsTask();
        return ExecuteCore(() => t);
    }
#endif

    /// <summary>
    /// Synchronously executes an asynchronous <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="task">The asynchronous <see cref="Task{TResult}"/> to execute.</param>
    /// <returns>A result of the executed task.</returns>
    public static TResult Execute<TResult>(Func<Task<TResult>> task)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        return ExecuteCore(task);
    }

    /// <summary>
    /// Synchronously executes a cancelable asynchronous <see cref="Task"/>.
    /// If the current thread is being aborted or interrupted then a cancellation is requested for the specified task.
    /// </summary>
    /// <param name="task">The cancelable asynchronous <see cref="Task"/> to execute.</param>
    public static void Execute(Func<CancellationToken, Task> task)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        ExecuteCore(task);
    }

    /// <summary>
    /// Synchronously executes a cancelable asynchronous <see cref="Task"/>.
    /// If the current thread is being aborted or interrupted then a cancellation is requested for the specified task.
    /// </summary>
    /// <param name="task">The cancelable asynchronous <see cref="Task"/> to execute.</param>
    /// <param name="cancellationToken">The additional cancellation token to propagate to the executed task.</param>
    public static void Execute(Func<CancellationToken, Task> task, CancellationToken cancellationToken)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        ExecuteCore(task, cancellationToken);
    }

    /// <summary>
    /// Synchronously executes an asynchronous <see cref="Task{TResult}"/>.
    /// If the current thread is being aborted or interrupted then a cancellation is requested for the specified task.
    /// </summary>
    /// <param name="task">The cancelable asynchronous <see cref="Task{TResult}"/> to execute.</param>
    /// <returns>A result of the executed task.</returns>
    public static TResult Execute<TResult>(Func<CancellationToken, Task<TResult>> task)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        return ExecuteCore(task, CancellationToken.None);
    }

    /// <summary>
    /// Synchronously executes an asynchronous <see cref="Task{TResult}"/>.
    /// If the current thread is being aborted or interrupted then a cancellation is requested for the specified task.
    /// </summary>
    /// <param name="task">The cancelable asynchronous <see cref="Task{TResult}"/> to execute.</param>
    /// <param name="cancellationToken">The additional cancellation token to propagate to the executed task.</param>
    /// <returns>A result of the executed task.</returns>
    public static TResult Execute<TResult>(Func<CancellationToken, Task<TResult>> task, CancellationToken cancellationToken)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(task);

        return ExecuteCore(task, cancellationToken);
    }

    /// <summary>
    /// Asynchronously executes a synchronous long-running action.
    /// </summary>
    /// <param name="action">The synchronous action to execute.</param>
    /// <returns>A <see cref="Task"/> that executes the specified action.</returns>
    public static Task ExecuteAsync(Action action) =>
        ExecuteAsyncCore(action ?? throw new ArgumentNullException(nameof(action)));

    /// <summary>
    /// Asynchronously executes a synchronous long-running function.
    /// </summary>
    /// <param name="func">The synchronous function to execute.</param>
    /// <returns>A <see cref="Task{TResult}"/> that executes the specified function.</returns>
    public static async Task<TResult> ExecuteAsync<TResult>(Func<TResult> func)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(func);

        var result = Optional<TResult>.None;

        await
            ExecuteAsyncCore(
                () =>
                {
                    result = func();
                })
            .ConfigureAwait(false);

        return result.Value;
    }

    /// <summary>
    /// Asynchronously executes a synchronous long-running action.
    /// If a cancellation is requested then a thread abort is issued for the execution thread of a synchronous function.
    /// </summary>
    /// <param name="action">The cancelable synchronous action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> that executes the specified action.</returns>
    public static Task ExecuteAsync(Action action, CancellationToken cancellationToken) =>
        ExecuteAsyncCore(
            action ?? throw new ArgumentNullException(nameof(action)),
            cancellationToken);

    /// <summary>
    /// Asynchronously executes a synchronous long-running function.
    /// If a cancellation is requested then a thread abort is issued for the execution thread of a synchronous function.
    /// </summary>
    /// <param name="func">The cancelable synchronous function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task{TResult}"/> that executes the specified function.</returns>
    public static async Task<TResult> ExecuteAsync<TResult>(Func<TResult> func, CancellationToken cancellationToken)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(func);

        var result = Optional<TResult>.None;

        await
            ExecuteAsyncCore(
                () =>
                {
                    result = func();
                },
                cancellationToken)
            .ConfigureAwait(false);

        return result.Value;
    }

    // ----------------------------------------------------------------------
    // Core Implementation
    // ----------------------------------------------------------------------

    static void ExecuteCore(Func<Task> task)
    {
        Debug.Assert(task is not null);

        bool disposeContext = true;
        var context = new ExclusiveSynchronizationContext();
        try
        {
            RunInContext(context, task, Cancel);
        }
        finally
        {
            if (disposeContext)
                context.Dispose();
        }

        void Cancel()
        {
            // Detach synchronization context from the current thread.
            disposeContext = false;

            void DetachedRun()
            {
                using (context)
                {
                    // Continue the task execution until its completion.
                    RunInContext(context, null, null);
                }
            }

            // Run the task in the background.
            StartLongRunningTask(DetachedRun);
        }
    }

    static TResult ExecuteCore<TResult>(Func<Task<TResult>> task)
    {
        Debug.Assert(task is not null);

        var result = Optional<TResult>.None;

        ExecuteCore(
            async () =>
            {
                result = await task().ConfigureAwait(false);
            });

        return result.Value;
    }

    static TResult ExecuteCore<TResult>(Func<CancellationToken, Task<TResult>> task, CancellationToken cancellationToken)
    {
        Debug.Assert(task is not null);

        var result = Optional<TResult>.None;

        ExecuteCore(
            async cancellationToken =>
            {
                result = await task(cancellationToken).ConfigureAwait(false);
            },
            cancellationToken);

        return result.Value;
    }

    static void ExecuteCore(Func<CancellationToken, Task> task, CancellationToken cancellationToken)
    {
        Debug.Assert(task is not null);

        using var cts = CancellationTokenSourceHelper.CreateLinked(cancellationToken);
        ExecuteCore(task, cts);
    }

    static void ExecuteCore(Func<CancellationToken, Task> task)
    {
        Debug.Assert(task is not null);

        using var cts = new CancellationTokenSource();
        ExecuteCore(task, cts);
    }

    static void ExecuteCore(Func<CancellationToken, Task> task, CancellationTokenSource cts)
    {
        Debug.Assert(task is not null);

        using var context = new ExclusiveSynchronizationContext();

        RunInContext(context, () => task(cts.Token), Cancel);

        void Cancel()
        {
            // Cancel the asynchronous task.
            cts.Cancel();

            // Execute remaining asynchronous operations following the task cancellation.
            context.Run();
        }
    }

    static Task ExecuteAsyncCore(Action action)
    {
        Debug.Assert(action is not null);

        return StartLongRunningTask(action);
    }

    static Task ExecuteAsyncCore(Action action, CancellationToken cancellationToken)
    {
        Debug.Assert(action is not null);

        // Use a quick path when possible.
        if (!cancellationToken.CanBeCanceled)
            return ExecuteAsyncCore(action);

        Task? executionTask = null;

        void Task()
        {
            // Use a graceful cancellation opportunity.
            cancellationToken.ThrowIfCancellationRequested();

            // Execute a cancelable synchronous action.
            try
            {
                using (cancellationToken.Register(
                    static state => ThreadHelper.TryCancel((Thread)state!),
                    Thread.CurrentThread))
                {
                    action();
                }
            }
            catch (ThreadAbortException)
            {
                // Allow the task to continue interacting with a task scheduler.
                ThreadHelper.TryResetAbort();

                throw new TaskCanceledException(Volatile.Read(ref executionTask));
            }
            catch (ThreadInterruptedException)
            {
                throw new TaskCanceledException(Volatile.Read(ref executionTask));
            }
        }

        var task = StartLongRunningTask(Task, cancellationToken);
        Volatile.Write(ref executionTask, task);

        return task;
    }

    static void RunInContext(ExclusiveSynchronizationContext context, Func<Task>? task, Action? cancel)
    {
        var savedContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(context);
        try
        {
            bool canceled = false;
            try
            {
                if (task != null)
                    context.Start(task);

                context.Run();
            }
            catch (ThreadInterruptedException) when (cancel != null)
            {
                cancel();
                canceled = true;
                throw;
            }
#if TFF_THREAD_ABORT
            catch (ThreadAbortException) when (cancel != null)
            {
                // Allow the task to continue interacting with a task scheduler.
                if (ThreadHelper.TryResetAbort())
                {
                    try
                    {
                        cancel();
                        canceled = true;
                    }
                    finally
                    {
                        // Restore the thread abort condition.
                        ThreadHelper.TryAbort(Thread.CurrentThread);
                    }
                }

                throw;
            }
#endif
            finally
            {
                if (!canceled)
                    context.Run();
            }
        }
        catch (AggregateException e) when (e.InnerExceptions.Count == 1)
        {
            throw e.InnerExceptions[0];
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(savedContext);
        }
    }

    static Task StartLongRunningTask(Action action, CancellationToken cancellationToken = default)
    {
        Debug.Assert(action is not null);

        // Running a synchronous action as a long-running task prevents thread pool pollution.
        // In this way, the task mostly acts as a standalone managed thread.
        return Task.Factory.StartNew(
            action,
            cancellationToken,
            TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
            TaskScheduler.Default);
    }
}
