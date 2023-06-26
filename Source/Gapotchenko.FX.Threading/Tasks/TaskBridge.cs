#if !(NETCOREAPP || NETSTANDARD2_1_OR_GREATER)
#define TFF_THREAD_ABORT
#endif

namespace Gapotchenko.FX.Threading.Tasks;

/// <summary>
/// Bridges synchronous and asynchronous task execution models together.
/// </summary>
public static class TaskBridge
{
    /// <summary>
    /// Synchronously executes an asynchronous task with a void return value.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    public static void Execute(Func<Task> task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        var savedContext = SynchronizationContext.Current;
        var context = new ExclusiveSynchronizationContext();
        try
        {
            SynchronizationContext.SetSynchronizationContext(context);
            context.Execute(task);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(savedContext);
        }
    }

    /// <summary>
    /// Synchronously completes execution of an already started asynchronous task.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    public static void Execute(Task task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        // Use a short path when possible.
        if (task.Status == TaskStatus.RanToCompletion)
        {
            // Task.Status property issues an acquire memory barrier internally.
            return;
        }

        Execute(() => task);
    }

#if TFF_VALUETASK
    /// <summary>
    /// Synchronously completes execution of an already started asynchronous task.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    public static void Execute(in ValueTask task)
    {
        // Use a short path when possible.
        if (task.IsCompletedSuccessfully)
        {
            // ValueTask.IsCompletedSuccessfully property issues an acquire memory barrier internally.
            return;
        }

        var t = task.AsTask();
        Execute(() => t);
    }
#endif

    /// <summary>
    /// Synchronously completes execution of an already started asynchronous task that returns a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>The return value.</returns>
    public static T Execute<T>(Task<T> task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        // Use a short path when possible.
        if (task.Status == TaskStatus.RanToCompletion)
        {
            // Task.Status property issues an acquire memory barrier internally.
            return task.Result;
        }

        return Execute(() => task);
    }

#if TFF_VALUETASK
    /// <summary>
    /// Synchronously completes execution of an already started asynchronous task that returns a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>The return value.</returns>
    public static T Execute<T>(in ValueTask<T> task)
    {
        // Use a short path when possible.
        if (task.IsCompleted)
        {
            // ValueTask<T>.IsCompleted property issues an acquire memory barrier internally.
            return task.Result;
        }

        var t = task.AsTask();
        return Execute(() => t);
    }
#endif

    /// <summary>
    /// Synchronously executes a cancelable asynchronous task with a void return value.
    /// If the current thread is being aborted or interrupted then a corresponding cancellation request is issued for the given task.
    /// </summary>
    /// <param name="task">The cancelable asynchronous task to execute.</param>
    public static void Execute(Func<CancellationToken, Task> task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        try
        {
            var savedContext = SynchronizationContext.Current;
            using var cts = new CancellationTokenSource();
            Task? pendingTask = null;
            var context = new ExclusiveSynchronizationContext();
            try
            {
                SynchronizationContext.SetSynchronizationContext(context);
                context.Execute(() => pendingTask = task(cts.Token));
                pendingTask = null;
            }
            catch (Exception ex) when (ex is ThreadAbortException || ex is ThreadInterruptedException)
            {
                cts.Cancel();

                if (pendingTask != null)
                {
                    context.ExceptionFilter = exception => exception is not TaskCanceledException;

                    // Execute remaining asynchronous iterations and finalizers.
                    context.Execute(() => pendingTask);
                }

                throw;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(savedContext);
            }
        }
        catch (AggregateException e) when (e.InnerExceptions.Count == 1)
        {
            throw e.InnerExceptions[0];
        }
    }

    /// <summary>
    /// Synchronously executes an asynchronous task with a return value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>The return value.</returns>
    public static T Execute<T>(Func<Task<T>> task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        T result = default!;
        Execute(
            async () =>
            {
                result = await task().ConfigureAwait(false);
            });

        return result!;
    }

    /// <summary>
    /// Synchronously executes an asynchronous task with a return value of type <typeparamref name="T"/>.
    /// If the current thread is being aborted or interrupted then a corresponding cancellation request is issued for the given task.
    /// </summary>
    /// <param name="task">The cancelable asynchronous task to execute.</param>
    /// <returns>The return value.</returns>
    public static T Execute<T>(Func<CancellationToken, Task<T>> task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        T result = default!;
        Execute(
            async ct =>
            {
                result = await task(ct).ConfigureAwait(false);
            });

        return result!;
    }

    static Task RunLongTask(Action action, CancellationToken cancellationToken) =>
        // Running a long task allows to avoid thread pool pollution on a large number of concurrent operations.
        Task.Factory.StartNew(
            action,
            cancellationToken,
            TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
            TaskScheduler.Default);

    /// <summary>
    /// Asynchronously executes a synchronous long-running action.
    /// </summary>
    /// <param name="action">The synchronous action to execute.</param>
    /// <returns>The task.</returns>
    public static Task ExecuteAsync(Action action) =>
        ExecuteAsyncCore(action ?? throw new ArgumentNullException(nameof(action)));

    static Task ExecuteAsyncCore(Action action) => RunLongTask(action, CancellationToken.None);

    /// <summary>
    /// Asynchronously executes a synchronous long-running function with a return value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="func">The synchronous function to execute.</param>
    /// <returns>The task.</returns>
    public static async Task<T> ExecuteAsync<T>(Func<T> func)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));

        T result = default!;
        await
            ExecuteAsync(
                () =>
                {
                    result = func();
                })
            .ConfigureAwait(false);

        return result!;
    }

    /// <summary>
    /// Asynchronously executes a synchronous and cancelable long-running action.
    /// If the asynchronous task is canceled via cancellation token then a thread abort is issued for the execution thread of a synchronous action.
    /// </summary>
    /// <param name="action">The cancelable synchronous action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    public static Task ExecuteAsync(Action action, CancellationToken cancellationToken) =>
        ExecuteAsyncCore(
            action ?? throw new ArgumentNullException(nameof(action)),
            cancellationToken);

    static Task ExecuteAsyncCore(Action action, CancellationToken cancellationToken)
    {
#if !TFF_THREAD_ABORT
        return RunLongTask(action, cancellationToken);
#else
        if (!cancellationToken.CanBeCanceled)
            return ExecuteAsyncCore(action);

        Thread? taskThread = null;
        using (cancellationToken.Register(
            () =>
            {
                var thread = taskThread;
                if (thread != null)
                {
                    try
                    {
                        thread.Abort();
                    }
                    catch (PlatformNotSupportedException)
                    {
                    }
                    catch (ThreadStateException)
                    {
                    }
                }
            },
            false))
        {
            return RunLongTask(
                () =>
                {
                    try
                    {
                        taskThread = Thread.CurrentThread;
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            action();
                        }
                        finally
                        {
                            taskThread = null;
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        try
                        {
                            Thread.ResetAbort();
                        }
                        catch (PlatformNotSupportedException)
                        {
                        }
                        throw new TaskCanceledException();
                    }
                },
                cancellationToken);
        }
#endif
    }

    /// <summary>
    /// Asynchronously executes a synchronous and cancelable long-running function with a return value of type <typeparamref name="T"/>.
    /// If the asynchronous task is canceled via cancellation token then a thread abort is issued for the execution thread of a synchronous function.
    /// </summary>
    /// <param name="func">The cancelable synchronous function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    public static async Task<T> ExecuteAsync<T>(Func<T> func, CancellationToken cancellationToken)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));

        T result = default!;
        await
            ExecuteAsync(
                () =>
                {
                    result = func();
                },
                cancellationToken)
            .ConfigureAwait(false);

        return result!;
    }
}
