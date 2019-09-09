using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Threading.Tasks
{
    /// <summary>
    /// Provides a drop-in replacement for <see cref="Parallel"/> class that by default performs a sequential execution of operations when the debugger is attached and parallel execution otherwise.
    /// <see cref="DebuggableParallel"/> is used for writing a debug-friendly code.
    /// </summary>
    public static class DebuggableParallel
    {
        /// <summary>
        /// Gets or sets the mode of operation.
        /// </summary>
        public static DebuggableParallelMode Mode { get; set; }

        static bool IsParallel
        {
            get
            {
                switch (Mode)
                {
                    default:
                    case DebuggableParallelMode.Auto:
                        return !Debugger.IsAttached;

                    case DebuggableParallelMode.AlwaysSequential:
                        return false;

                    case DebuggableParallelMode.AwaysParallel:
                        return true;
                }
            }
        }

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
        /// Executes each of the provided actions sequentially when the debugger is attached or in parallel otherwise.
        /// </summary>
        /// <param name="actions">An array of <see cref="Action"/> to execute.</param>
        public static void Invoke(params Action[] actions)
        {
            if (IsParallel)
                Parallel.Invoke(actions);
            else
                Sequential.Invoke(actions);
        }
    }
}
