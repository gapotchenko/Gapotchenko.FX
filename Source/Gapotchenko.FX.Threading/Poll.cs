// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides primitives for efficient polling.
/// </summary>
public static class Poll
{
    /// <summary>
    /// Asynchronously waits for a condition to come true by executing an efficient polling strategy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method can be used to turn a polling operation to an asynchronous wait task.
    /// </para>
    /// <para>
    /// The efficiency of the polling strategy is achieved by randomizing delays between polling attempts.
    /// In this way, a peak pressure on a thread pool is minimized by dispersing thread activities in time.
    /// The randomization also lowers the statistical rate of collisions between concurrent resource accessors.
    /// </para>
    /// </remarks>
    /// <param name="condition">The asynchronous predicate which defines the condition to wait for.</param>
    /// <param name="millisecondsInterval">The amount of time, in milliseconds, to wait between condition polling attempts.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the condition to come true.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> when the wait condition is met in time;
    /// <see langword="false"/> when the operation is timed out.</returns>
    public static async Task<bool> WaitUntilAsync(
        Func<Task<bool>> condition,
        int millisecondsInterval,
        int millisecondsTimeout,
        CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(condition);
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

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
