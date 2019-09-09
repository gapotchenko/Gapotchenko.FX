using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Threading.Tasks
{
    /// <summary>
    /// Provides a drop-in replacement for <see cref="Parallel"/> class that performs a sequential execution of operations instead of parallel.
    /// Such a replacement is useful for debugging purposes.
    /// </summary>
    public static class Sequential
    {
        /// <summary>
        /// Executes a <c>foreach</c> (<c>For Each</c> in Visual Basic) operation on an <see cref="IEnumerable{T}"/> in which iterations are run sequentially.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            foreach (var i in source)
                body(i);
        }

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
    }
}
