﻿using System.Collections.Concurrent;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Tasks;

/// <summary>
/// Provides a drop-in replacement for <see cref="Parallel"/> class that by default performs a sequential execution of operations when a debugger is attached,
/// or a parallel execution when no debugger is attached.
/// </summary>
/// <remarks>
/// <see cref="DebuggableParallel"/> class can be used for writing debug-friendly code that has a deterministic and easy to debug behavior when a debugger is attached.
/// </remarks>
public static class DebuggableParallel
{
    /// <summary>
    /// Gets or sets the mode of operation.
    /// </summary>
    public static DebuggableParallelMode Mode { get; set; }

    /// <summary>
    /// Gets a value indicating whether to perform a parallel execution of operations in the current context.
    /// </summary>
    /// <remarks>
    /// By default, operations are executed in parallel when no debugger is attached, and sequentially when a debugger is attached.
    /// </remarks>
    public static bool IsParallel =>
        Mode switch
        {
            DebuggableParallelMode.AlwaysSequential => false,
            DebuggableParallelMode.AlwaysParallel => true,
            DebuggableParallelMode.Auto => !Debugger.IsAttached
        };

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with thread-local data in which iterations are run
    /// sequentially when a debugger is attached,
    /// or in parallel when no debugger is attached.
    /// The state of the loop can be monitored and manipulated.
    /// </summary>
    /// <inheritdoc cref="Parallel.For{TLocal}(int, int, Func{TLocal}, Func{int, ParallelLoopState, TLocal, TLocal}, Action{TLocal})"/>
    public static ParallelLoopResult For<TLocal>(
        int fromInclusive,
        int toExclusive,
        Func<TLocal> localInit,
        Func<int, ParallelLoopState, TLocal, TLocal> body,
        Action<TLocal> localFinally) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, localInit, body, localFinally) :
            Sequential.For(fromInclusive, toExclusive, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes and thread-local data in which iterations are run
    /// sequentially when a debugger is attached,
    /// or in parallel when no debugger is attached.
    /// The state of the loop can be monitored and manipulated.
    /// </summary>
    /// <inheritdoc cref="Parallel.For{TLocal}(long, long, Func{TLocal}, Func{long, ParallelLoopState, TLocal, TLocal}, Action{TLocal})"/>
    public static ParallelLoopResult For<TLocal>(
        long fromInclusive,
        long toExclusive,
        Func<TLocal> localInit,
        Func<long, ParallelLoopState, TLocal, TLocal> body,
        Action<TLocal> localFinally) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, localInit, body, localFinally) :
            Sequential.For(fromInclusive, toExclusive, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with thread-local data in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, parallelOptions, localInit, body, localFinally) :
            Sequential.For(fromInclusive, toExclusive, parallelOptions, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes and thread-local data in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, parallelOptions, localInit, body, localFinally) :
            Sequential.For(fromInclusive, toExclusive, parallelOptions, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop in which iterations are run sequentially when the debugger is attached or in parallel otherwise.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(int fromInclusive, int toExclusive, Action<int> body) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, body) :
            Sequential.For(fromInclusive, toExclusive, body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes in which iterations are run sequentially when the debugger is attached or in parallel otherwise.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(long fromInclusive, long toExclusive, Action<long> body) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, body) :
            Sequential.For(fromInclusive, toExclusive, body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// and loop options can be configured.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(int fromInclusive, int toExclusive, ParallelOptions parallelOptions, Action<int> body) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, parallelOptions, body) :
            Sequential.For(fromInclusive, toExclusive, parallelOptions, body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// and loop options can be configured.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(long fromInclusive, long toExclusive, ParallelOptions parallelOptions, Action<long> body) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, parallelOptions, body) :
            Sequential.For(fromInclusive, toExclusive, parallelOptions, body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(int fromInclusive, int toExclusive, Action<int, ParallelLoopState> body) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, body) :
            Sequential.For(fromInclusive, toExclusive, body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(long fromInclusive, long toExclusive, Action<long, ParallelLoopState> body) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, body) :
            Sequential.For(fromInclusive, toExclusive, body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(int fromInclusive, int toExclusive, ParallelOptions parallelOptions, Action<int, ParallelLoopState> body) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, parallelOptions, body) :
            Sequential.For(fromInclusive, toExclusive, parallelOptions, body);

    /// <summary>
    /// Executes a <c>for</c> (<c>For</c> in Visual Basic) loop with 64-bit indexes in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <param name="fromInclusive">The start index, inclusive.</param>
    /// <param name="toExclusive">The end index, exclusive.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult For(long fromInclusive, long toExclusive, ParallelOptions parallelOptions, Action<long, ParallelLoopState> body) =>
        IsParallel ?
            Parallel.For(fromInclusive, toExclusive, parallelOptions, body) :
            Sequential.For(fromInclusive, toExclusive, parallelOptions, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) =>
        IsParallel ?
            Parallel.ForEach(source, body) :
            Sequential.ForEach(source, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource> body) =>
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, body) :
            Sequential.ForEach(source, parallelOptions, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource, ParallelLoopState> body) =>
        IsParallel ?
            Parallel.ForEach(source, body) :
            Sequential.ForEach(source, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState> body) =>
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, body) :
            Sequential.ForEach(source, parallelOptions, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with 64-bit indexes on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource, ParallelLoopState, long> body) =>
        IsParallel ?
            Parallel.ForEach(source, body) :
            Sequential.ForEach(source, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with 64-bit indexes on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState, long> body) =>
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, body) :
            Sequential.ForEach(source, parallelOptions, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.ForEach(source, localInit, body, localFinally) :
            Sequential.ForEach(source, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, localInit, body, localFinally) :
            Sequential.ForEach(source, parallelOptions, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data and 64-bit indexes on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.ForEach(source, localInit, body, localFinally) :
            Sequential.ForEach(source, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data and 64-bit indexes on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, localInit, body, localFinally) :
            Sequential.ForEach(source, parallelOptions, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// and loop options can be configured.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, Action<TSource> body) =>
        IsParallel ?
            Parallel.ForEach(source, body) :
            Sequential.ForEach(source, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// and loop options can be configured.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, ParallelOptions parallelOptions, Action<TSource> body) =>
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, body) :
            Sequential.ForEach(source, parallelOptions, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, Action<TSource, ParallelLoopState> body) =>
        IsParallel ?
            Parallel.ForEach(source, body) :
            Sequential.ForEach(source, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The partitioner that contains the original data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState> body) =>
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, body) :
            Sequential.ForEach(source, parallelOptions, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.ForEach(source, localInit, body, localFinally) :
            Sequential.ForEach(source, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with thread-local data on a <see cref="Partitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, localInit, body, localFinally) :
            Sequential.ForEach(source, parallelOptions, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="OrderablePartitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The orderable partitioner that contains the original data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(OrderablePartitioner<TSource> source, Action<TSource, ParallelLoopState, long> body) =>
        IsParallel ?
            Parallel.ForEach(source, body) :
            Sequential.ForEach(source, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on a <see cref="OrderablePartitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
    /// loop options can be configured,
    /// and the state of the loop can be monitored and manipulated.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in source.</typeparam>
    /// <param name="source">The orderable partitioner that contains the original data source.</param>
    /// <param name="parallelOptions">An object that configures the behavior of this operation.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <returns>A structure that contains information about which portion of the loop completed.</returns>
    public static ParallelLoopResult ForEach<TSource>(OrderablePartitioner<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState, long> body) =>
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, body) :
            Sequential.ForEach(source, parallelOptions, body);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with 64-bit indexes and with thread-local data on a <see cref="OrderablePartitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.ForEach(source, localInit, body, localFinally) :
            Sequential.ForEach(source, localInit, body, localFinally);

    /// <summary>
    /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation with 64-bit indexes and with thread-local data on a <see cref="OrderablePartitioner{TSource}"/> in which iterations are run sequentially when the debugger is attached or in parallel otherwise,
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
        IsParallel ?
            Parallel.ForEach(source, parallelOptions, localInit, body, localFinally) :
            Sequential.ForEach(source, parallelOptions, localInit, body, localFinally);

    /// <summary>
    /// Executes each of the provided actions,
    /// sequentially when a debugger is attached,
    /// or in parallel when no debugger is attached.
    /// </summary>
    /// <inheritdoc cref="Parallel.Invoke(Action[])"/>
    public static void Invoke(params Action[] actions)
    {
        if (IsParallel)
            Parallel.Invoke(actions);
        else
            Sequential.Invoke(actions);
    }

    /// <summary>
    /// Executes each of the provided actions,
    /// sequentially when a debugger is attached,
    /// or in parallel when no debugger is attached,
    /// unless the operation is cancelled by the user.
    /// </summary>
    /// <inheritdoc cref="Parallel.Invoke(ParallelOptions, Action[])"/>
    public static void Invoke(ParallelOptions parallelOptions, params Action[] actions)
    {
        if (IsParallel)
            Parallel.Invoke(parallelOptions, actions);
        else
            Sequential.Invoke(parallelOptions, actions);
    }
}
