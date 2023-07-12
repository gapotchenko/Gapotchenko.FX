// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Utils;

partial class TaskHelper
{
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
        return DoExecuteWithTimeoutAsync(
            func,
            timeout,
            Optional<TResult>.None,
            cancellationToken);
    }

    /// <summary>
    /// Executes the specified asynchronous function during the specified timeout.
    /// If the timeout expires before the function completes, it will be canceled
    /// and the method will return the specified timeout result.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="timeoutResult">The result to return on timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A result of <paramref name="func"/> execution if it completes before <paramref name="timeout"/> expires,
    /// or <paramref name="timeoutResult"/> if the timeout expires before the function completes.
    /// </returns>
    /// <exception cref="TimeoutException">The operation has timed out.</exception>
    public static Task<TResult> ExecuteWithTimeoutAsync<TResult>(
        Func<CancellationToken, Task<TResult>> func,
        TimeSpan timeout,
        TResult timeoutResult,
        CancellationToken cancellationToken = default)
    {
        return DoExecuteWithTimeoutAsync(
            func,
            timeout,
            timeoutResult,
            cancellationToken);
    }

    static Task<TResult> DoExecuteWithTimeoutAsync<TResult>(
        Func<CancellationToken, Task<TResult>> func,
        TimeSpan timeout,
        Optional<TResult> timeoutResult,
        CancellationToken cancellationToken)
    {
        if (timeout == Timeout.InfiniteTimeSpan)
            return func(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<TResult>(cancellationToken);

        if (timeout == TimeSpan.Zero)
        {
            if (timeoutResult.HasValue)
                return Task.FromResult(timeoutResult.Value);
            else
                return Task.FromException<TResult>(new TimeoutException());
        }

        async Task<TResult> ExecuteAsync()
        {
            using var cts = new CancellationTokenSource(timeout);

            // Link the cancellation token source with the user-supplied token.
            using var ctr = cancellationToken.Register(cts.Cancel);

            try
            {
                return await func(cts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (cts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    // Timeout is expired.
                    if (timeoutResult.HasValue)
                        return timeoutResult.Value;
                    else
                        throw new TimeoutException();
                }
                else
                {
                    throw;
                }
            }
        }

        return ExecuteAsync();
    }
}
