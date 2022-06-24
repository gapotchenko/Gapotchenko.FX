using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides efficient primitives for polling.
/// </summary>
public static class Poll
{
    /// <summary>
    /// Asynchronously waits for a condition to come true by executing an efficient polling strategy.
    /// This method can be used to bridge a poll operation to an asynchronous wait task.
    /// </summary>
    /// <remarks>
    /// The efficiency of a polling strategy is achieved by randomizing the delays between the polling attempts.
    /// In this way, the peak pressure on a thread pool is minimized by dispersing the thread activation requests on time axis.
    /// </remarks>
    /// <param name="condition">The asynchronous predicate which defines the condition to wait for.</param>
    /// <param name="millisecondsInterval">The amount of time, in milliseconds, to wait between condition polling attempts.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the condition to come true.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> when the wait condition is met; <c>false</c> when operation is timed out.</returns>
    public static async Task<bool> WaitUntilAsync(
        Func<Task<bool>> condition,
        int millisecondsInterval,
        int millisecondsTimeout,
        CancellationToken cancellationToken = default)
    {
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));
        if (millisecondsTimeout < Timeout.Infinite)
            throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.");

        Stopwatch? sw = null;

        int minDispersion = 0;
        int maxDispersion = 0;
        var random = Optional<Random?>.None;

        for (; ; )
        {
            if (await condition().ConfigureAwait(false))
                return true;

            if (sw != null)
            {
                if (sw.ElapsedMilliseconds > millisecondsTimeout)
                    return false;
            }
            else if (millisecondsTimeout != Timeout.Infinite)
            {
                sw = Stopwatch.StartNew();
            }

            if (!random.HasValue)
            {
                int dispersion = millisecondsInterval / 5;
                if (dispersion > 0)
                {
                    random = new Random();
                    minDispersion = -dispersion;
                    maxDispersion = dispersion + 1;
                }
                else
                {
                    random = null;
                }
            }

            int millisecondsDelay = millisecondsInterval;

            var rv = random.Value;
            if (rv != null)
            {
                int dispersion = rv.Next(minDispersion, maxDispersion);
                millisecondsDelay += dispersion;
            }

            await Task.Delay(millisecondsDelay, cancellationToken).ConfigureAwait(false);
        }
    }
}
