using System.Collections.Concurrent;
using System.Diagnostics;

#nullable disable

namespace Gapotchenko.FX.Threading.Tasks;

/// <summary>
/// Provides a drop-in replacement for <see cref="Parallel"/> class that performs a sequential execution of operations instead of parallel.
/// Such a replacement is useful for debugging purposes.
/// </summary>
public static class Sequential
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static readonly ParallelOptions m_SequentialParallelOptions = new() { MaxDegreeOfParallelism = 1 };

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with thread-local data in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <inheritdoc cref="Parallel.For{TLocal}(int, int, Func{TLocal}, Func{int, ParallelLoopState, TLocal, TLocal}, Action{TLocal})"/>
    public static ParallelLoopResult For<TLocal>(int fromInclusive, int toExclusive, Func<TLocal> localInit, Func<int, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            m_SequentialParallelOptions,
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes and thread-local data in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For<TLocal>(long fromInclusive, long toExclusive, Func<TLocal> localInit, Func<long, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            m_SequentialParallelOptions,
            localInit,
            body,
            localFinally);

    static ParallelOptions MakeSequential(ParallelOptions parallelOptions)
    {
        if (parallelOptions == null)
            return null;

        int maxDegreeOfParallelism = parallelOptions.MaxDegreeOfParallelism;
        if (maxDegreeOfParallelism <= 1 && maxDegreeOfParallelism != -1)
        {
            // Already sequential or out of range.
            return parallelOptions;
        }
        else if (parallelOptions.TaskScheduler == null && !parallelOptions.CancellationToken.CanBeCanceled)
        {
            // Avoid object allocation.
            return m_SequentialParallelOptions;
        }
        else
        {
            // Limit max degree of parallelism to sequential.
            return new ParallelOptions
            {
                TaskScheduler = parallelOptions.TaskScheduler,
                CancellationToken = parallelOptions.CancellationToken,
                MaxDegreeOfParallelism = 1
            };
        }
    }

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with thread-local data in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For<TLocal>(int fromInclusive, int toExclusive, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<int, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            MakeSequential(parallelOptions),
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes and thread-local data in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For<TLocal>(long fromInclusive, long toExclusive, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<long, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            MakeSequential(parallelOptions),
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop in which iterations are run sequentially
    /// and loop options can be configured,
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(int fromInclusive, int toExclusive, Action<int> body) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes in which iterations are run sequentially
    /// and loop options can be configured.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(long fromInclusive, long toExclusive, Action<long> body) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop in which iterations are run sequentially
    /// and loop options can be configured.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(int fromInclusive, int toExclusive, ParallelOptions parallelOptions, Action<int> body) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes in which iterations are run sequentially
    /// and loop options can be configured,
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(long fromInclusive, long toExclusive, ParallelOptions parallelOptions, Action<long> body) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(int fromInclusive, int toExclusive, Action<int, ParallelLoopState> body) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(long fromInclusive, long toExclusive, Action<long, ParallelLoopState> body) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(int fromInclusive, int toExclusive, ParallelOptions parallelOptions, Action<int, ParallelLoopState> body) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(long fromInclusive, long toExclusive, ParallelOptions parallelOptions, Action<long, ParallelLoopState> body) =>
        Parallel.For(
            fromInclusive,
            toExclusive,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially
    /// and loop options can be configured.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially,
    /// and loop options can be configured.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource> body) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource, ParallelLoopState> body) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState> body) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with 64-bit indexes on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource, ParallelLoopState, long> body) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with 64-bit indexes on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState, long> body) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, Func<TLocal> localInit, Func<TSource, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data and 64-bit indexes on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, Func<TLocal> localInit, Func<TSource, ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data and 64-bit indexes on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially
    /// and loop options can be configured.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, Action<TSource> body) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially
    /// and loop options can be configured.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, ParallelOptions parallelOptions, Action<TSource> body) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, Action<TSource, ParallelLoopState> body) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState> body) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource, TLocal>(Partitioner<TSource> source, Func<TLocal> localInit, Func<TSource, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource, TLocal>(Partitioner<TSource> source, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="OrderablePartitioner{TSource}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The orderable partitioner that contains the original data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(OrderablePartitioner<TSource> source, Action<TSource, ParallelLoopState, long> body) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="OrderablePartitioner{TSource}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The orderable partitioner that contains the original data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(OrderablePartitioner<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState, long> body) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with 64-bit indexes and with thread-local data on a <see cref="OrderablePartitioner{TSource}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="source">The orderable partitioner that contains the original data source.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource, TLocal>(OrderablePartitioner<TSource> source, Func<TLocal> localInit, Func<TSource, ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.ForEach(
            source,
            m_SequentialParallelOptions,
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with 64-bit indexes and with thread-local data on a <see cref="OrderablePartitioner{TSource}"/> in which iterations are run sequentially,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <typeparam name="TLocal">The type of the thread-local data.</typeparam>
    /// <param name="source">The orderable partitioner that contains the original data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="localInit">The function delegate that returns the initial state of the local data for each task.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <param name="localFinally">The delegate that performs a final action on the local state of each task.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource, TLocal>(OrderablePartitioner<TSource> source, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally) =>
        Parallel.ForEach(
            source,
            MakeSequential(parallelOptions),
            localInit,
            body,
            localFinally);

    /// <summary>
    /// Executes each of the provided actions, sequentially.
    /// </summary>
    /// <param name="actions">An array of <see cref="Action"/> to execute.</param>
    public static void Invoke(params Action[] actions)
    {
        if (actions == null)
            throw new ArgumentNullException(nameof(actions));

        foreach (var action in actions)
            action();
    }

    /// <summary>
    /// Executes each of the provided actions, sequentially.
    /// </summary>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="actions">An array of <see cref="Action"/> to execute.</param>
    public static void Invoke(ParallelOptions parallelOptions, params Action[] actions) =>
        Parallel.Invoke(
            MakeSequential(parallelOptions),
            actions);
}
