using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Threading.Tasks;
using System;
using System.Linq;

namespace Gapotchenko.FX.Linq;

/// <summary>
/// Provides an extended set of methods for querying objects that implement <see cref="ParallelQuery{TSource}"/>
/// </summary>
public static class ParallelEnumerableEx
{
    /// <summary>
    /// Enables debugging-friendly execution of a parallelized query by running it sequentially when the debugger is attached or in parallel otherwise.
    /// </summary>
    /// <typeparam name="TSource">The type of elements of source.</typeparam>
    /// <param name="source">A <see cref="ParallelQuery{TSource}"/> on which to set a debug-friendly limit on the degrees of parallelism.</param>
    /// <returns><see cref="ParallelQuery{TSource}"/> representing the same query as <paramref name="source"/>, with the debugging-friendly limit on the degrees of parallelism set.</returns>
    public static ParallelQuery<TSource> AsDebuggable<TSource>(this ParallelQuery<TSource> source) =>
        DebuggableParallel.IsParallel ?
            source ?? throw new ArgumentNullException(nameof(source)) :
            source.AsParallel().WithDegreeOfParallelism(1);

    /// <summary>
    /// Sets the maximum degree of parallelism to use in a query.
    /// Maximum degree of parallelism is the upper limit of concurrently executing processor tasks that can be used to process the query.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source">A <see cref="ParallelQuery{TSource}"/> on which to set the upper limit on the degrees of parallelism.</param>
    /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism.</param>
    /// <returns><see cref="ParallelQuery{TSource}"/> representing the same query as <paramref name="source"/>, with the upper limit on the degrees of parallelism set.</returns>
    public static ParallelQuery<TSource> WithMaxDegreeOfParallelism<TSource>(this ParallelQuery<TSource> source, int maxDegreeOfParallelism)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (maxDegreeOfParallelism == 0 || maxDegreeOfParallelism < -1)
            throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism));

        int degreeOfParallelism;
        if (maxDegreeOfParallelism == 1)
        {
            degreeOfParallelism = 1;
        }
        else
        {
            int processCount = ThreadingCapabilities.LogicalProcessorCount;
            degreeOfParallelism =
                maxDegreeOfParallelism == -1 ?
                    processCount :
                    Math.Min(processCount, maxDegreeOfParallelism);
        }

        return source.WithDegreeOfParallelism(degreeOfParallelism);
    }
}
