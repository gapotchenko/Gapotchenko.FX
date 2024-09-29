// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <summary>
    /// The current version of the graph which allows to detect modifications.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Version;

    /// <summary>
    /// Increments the current version of the graph which allows to detect modifications.
    /// </summary>
    void IncrementVersion() => ++m_Version;

    readonly struct ModificationGuard(Graph<TVertex> graph)
    {
        /// <summary>
        /// Protects <see cref="IEnumerable{T}"/> against graph modifications.
        /// </summary>
        [return: NotNullIfNotNull(nameof(source))]
        public IEnumerable<TVertex>? Protect(IEnumerable<TVertex>? source) =>
            source switch
            {
                null => null,
                var x when ReferenceEquals(x, Enumerable.Empty<TVertex>()) => x,
                _ => ProtectCore(source)
            };

        IEnumerable<TVertex> ProtectCore(IEnumerable<TVertex> source)
        {
            foreach (var i in source)
            {
                Checkpoint();
                yield return i;
            }
        }

        /// <summary>
        /// Ensures that the graph hasn't been modified since the moment the current guard was initialized.
        /// </summary>
        [StackTraceHidden]
        public void Checkpoint()
        {
            if (graph.m_Version != m_Version)
                Throw();
        }

        readonly int m_Version = graph.m_Version;

        /// <summary>
        /// Throws a graph modification exception.
        /// </summary>
        [DoesNotReturn, StackTraceHidden]
        public static void Throw() =>
            throw new InvalidOperationException("Graph was modified; enumeration operation may not execute.");
    }
}
