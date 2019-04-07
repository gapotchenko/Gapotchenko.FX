using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Threading.Tasks
{
    /// <summary>
    /// Bridges together synchronous and asynchronous task execution models.
    /// </summary>
    public static class TaskBridge
    {
        /// <summary>
        /// Synchronously executes an async task with a void return value.
        /// </summary>
        /// <param name="task">The async task to execute.</param>
        public static void Execute(Func<Task> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var savedContext = SynchronizationContext.Current;
            var context = new ExclusiveSynchronizationContext();
            try
            {
                SynchronizationContext.SetSynchronizationContext(context);
                context.ExecuteAsyncTaskSynchronously(task);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(savedContext);
            }
        }

        /// <summary>
        /// Synchronously executes an async task with a void return value.
        /// </summary>
        /// <param name="task">The async task to execute.</param>
        public static void Execute(Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                return;

            Execute(() => task);
        }

        /// <summary>
        /// Synchronously executes a cancelable async task with a void return value.
        /// If the current thread is being aborted or interrupted then a corresponding cancellation request is issued for the given task.
        /// </summary>
        /// <param name="task">The cancelable async task to execute.</param>
        public static void Execute(Func<CancellationToken, Task> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            try
            {
                var savedContext = SynchronizationContext.Current;
                using (var cts = new CancellationTokenSource())
                {
                    Task pendingTask = null;
                    var context = new ExclusiveSynchronizationContext();
                    try
                    {
                        SynchronizationContext.SetSynchronizationContext(context);
                        context.ExecuteAsyncTaskSynchronously(() => pendingTask = task(cts.Token));
                        pendingTask = null;
                    }
                    catch (Exception ex) when (ex is ThreadAbortException || ex is ThreadInterruptedException)
                    {
                        cts.Cancel();

                        if (pendingTask != null)
                        {
                            context.InnerExceptionFilter = exception => !(exception is TaskCanceledException);

                            // Execute remaining async iterations and finalizers.
                            context.ExecuteAsyncTaskSynchronously(() => pendingTask);
                        }

                        throw;
                    }
                    finally
                    {
                        SynchronizationContext.SetSynchronizationContext(savedContext);
                    }
                }
            }
            catch (AggregateException e) when (e.InnerExceptions.Count == 1)
            {
                throw e.InnerExceptions[0];
            }
        }

        /// <summary>
        /// Synchronously executes an async task with a return value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="task">The async task to execute.</param>
        /// <returns>The return value.</returns>
        public static T Execute<T>(Func<Task<T>> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var result = default(T);
            Execute(
                async () =>
                {
                    result = await task().ConfigureAwait(false);
                });
            return result;
        }

        /// <summary>
        /// Synchronously executes an async task with a return value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="task">The async task to execute.</param>
        /// <returns>The return value.</returns>
        public static T Execute<T>(Task<T> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                return task.Result;

            return Execute(() => task);
        }

        /// <summary>
        /// Synchronously executes an async task with a return value of type <typeparamref name="T"/>.
        /// If the current thread is being aborted or interrupted then a corresponding cancellation request is issued for the given task.
        /// </summary>
        /// <param name="task">The cancelable async task to execute.</param>
        /// <returns>The return value.</returns>
        public static T Execute<T>(Func<CancellationToken, Task<T>> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var result = default(T);
            Execute(
                async ct =>
                {
                    result = await task(ct).ConfigureAwait(false);
                });
            return result;
        }

        static Task _RunLongTask(Action action, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(
                action,
                cancellationToken,
#if NET40
                TaskCreationOptions.LongRunning,
#else
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
#endif
                TaskScheduler.Default);
        }

        /// <summary>
        /// Asynchronously executes a synchronous long-running action.
        /// </summary>
        /// <param name="action">The synchronous action to execute.</param>
        /// <returns>The task.</returns>
        public static Task ExecuteAsync(Action action) =>
            _ExecuteAsyncCore(action ?? throw new ArgumentNullException(nameof(action)));

        static Task _ExecuteAsyncCore(Action action) => _RunLongTask(action, CancellationToken.None);

        /// <summary>
        /// Asynchronously executes a synchronous long-running function with a return value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="func">The synchronous function to execute.</param>
        /// <returns>The task.</returns>
        public static async Task<T> ExecuteAsync<T>(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var result = default(T);
            await
                ExecuteAsync(
                    () =>
                    {
                        result = func();
                    })
                .ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Asynchronously executes a synchronous and cancelable long-running action.
        /// If the asynchronous task is canceled via cancellation token then a thread abort is issued for the execution thread of a synchronous action.
        /// </summary>
        /// <param name="action">The cancelable synchronous action to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        public static Task ExecuteAsync(Action action, CancellationToken cancellationToken)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (!cancellationToken.CanBeCanceled)
                return _ExecuteAsyncCore(action);
            else
                return _ExecuteAsyncCore(action, cancellationToken);
        }

        static Task _ExecuteAsyncCore(Action action, CancellationToken cancellationToken)
        {
            Thread taskThread = null;
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
                        catch (ThreadStateException)
                        {
                        }
                    }
                },
                false))
            {
                return
                    _RunLongTask(
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
                                Thread.ResetAbort();
                                throw new TaskCanceledException();
                            }
                        },
                        cancellationToken);
            }
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

            var result = default(T);
            await
                ExecuteAsync(
                    () =>
                    {
                        result = func();
                    },
                    cancellationToken)
                .ConfigureAwait(false);
            return result;
        }
    }
}
