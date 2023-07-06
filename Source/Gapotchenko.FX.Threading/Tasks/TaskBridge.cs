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
    /// Synchronously completes the execution of an already started asynchronous <see cref="Task"/>.
    /// </summary>
    /// <param name="task">The asynchronous <see cref="Task"/> to execute.</param>
    public static void Execute(Task task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

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
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        ExecuteCore(task);
    }

    /// <summary>
    /// Synchronously completes the execution of an already started asynchronous <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="task">The asynchronous <see cref="Task{TResult}"/> to execute.</param>
    /// <returns>A result of the executed task.</returns>
    public static TResult Execute<TResult>(Task<TResult> task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        // Use a quick path when possible.
        if (task.IsCompleted)
            return task.GetAwaiter().GetResult();

        return ExecuteCore(() => task);
    }

#if TFF_VALUETASK
    /// <summary>
    /// Synchronously completes the execution of an already started asynchronous <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="task">The asynchronous <see cref="Task{TResult}"/> to execute.</param>
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
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        return ExecuteCore(task);
    }

    /// <summary>
    /// Synchronously executes a cancelable asynchronous <see cref="Task"/>.
    /// If the current thread is being aborted or interrupted then a cancellation is requested for the specified task.
    /// </summary>
    /// <param name="task">The cancelable asynchronous <see cref="Task"/> to execute.</param>
    public static void Execute(Func<CancellationToken, Task> task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        ExecuteCore(task);
    }

    /// <summary>
    /// Synchronously executes an asynchronous <see cref="Task{TResult}"/>.
    /// If the current thread is being aborted or interrupted then a cancellation is requested for the specified task.
    /// </summary>
    /// <param name="task">The cancelable asynchronous <see cref="Task{TResult}"/> to execute.</param>
    /// <returns>A result of the executed task.</returns>
    public static TResult Execute<TResult>(Func<CancellationToken, Task<TResult>> task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        var result = Optional<TResult>.None;

        ExecuteCore(
            async cancellationToken =>
            {
                result = await task(cancellationToken).ConfigureAwait(false);
            });

        return result.Value;
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
        if (func == null)
            throw new ArgumentNullException(nameof(func));

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
    /// Asynchronously executes a synchronous and cancelable long-running action.
    /// If the asynchronous task is canceled via cancellation token then a thread abort is issued for the execution thread of a synchronous action.
    /// </summary>
    /// <param name="action">The cancelable synchronous action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> that executes the specified action.</returns>
    public static Task ExecuteAsync(Action action, CancellationToken cancellationToken) =>
        ExecuteAsyncCore(
            action ?? throw new ArgumentNullException(nameof(action)),
            cancellationToken);

    /// <summary>
    /// Asynchronously executes a synchronous and cancelable long-running function.
    /// If the asynchronous task is canceled via cancellation token then a thread abort is issued for the execution thread of a synchronous function.
    /// </summary>
    /// <param name="func">The cancelable synchronous function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task{TResult}"/> that executes the specified function.</returns>
    public static async Task<TResult> ExecuteAsync<TResult>(Func<TResult> func, CancellationToken cancellationToken)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));

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

    static void ExecuteCore(Func<Task> task)
    {
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

    static TResult ExecuteCore<TResult>(Func<Task<TResult>> task)
    {
        var result = Optional<TResult>.None;

        ExecuteCore(
            async () =>
            {
                result = await task().ConfigureAwait(false);
            });

        return result.Value;
    }

    static void ExecuteCore(Func<CancellationToken, Task> task)
    {
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

    static Task ExecuteAsyncCore(Action action) => RunLongTask(action, CancellationToken.None);

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

    static Task RunLongTask(Action action, CancellationToken cancellationToken) =>
        // Running a synchronous action in a long-running task prevents thread pool pollution.
        // In this way, task acts as a standalone thread.
        Task.Factory.StartNew(
            action,
            cancellationToken,
            TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
            TaskScheduler.Default);
}
