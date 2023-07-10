// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Utils;

static class TaskHelper
{
    /// <summary>
    /// Represents a non-existing task result.
    /// </summary>
    public struct VoidResult
    {
    }

    public static void EnqueueDisposeOnCompletion(Task task, IDisposable disposable)
    {
        task.ContinueWith(
            _ => disposable.Dispose(),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default);
    }

    /// <summary>
    /// Executes the specified asynchronous function during the specified timeout.
    /// If the timeout expires before the function completes, it will be canceled
    /// and the method will throw a <see cref="TimeoutException"/>.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A function result.</returns>
    /// <exception cref="TimeoutException">The operation has timed out.</exception>
    public static Task<TResult> ExecuteWithTimeoutAsync<TResult>(
        Func<CancellationToken, Task<TResult>> func,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        if (timeout == Timeout.InfiniteTimeSpan)
            return func(cancellationToken);

        if (timeout == TimeSpan.Zero)
            return Task.FromException<TResult>(new TimeoutException());

        async Task<TResult> ExecuteAsync()
        {
            var cts = new CancellationTokenSource(timeout);

            // Link the cancellation token source with the user-supplied token.
            using var ctr = cancellationToken.Register(cts.Cancel);

            try
            {
                return await func(cts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (cts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                    throw new TimeoutException();
                else
                    throw;
            }
        }

        return ExecuteAsync();
    }
}
