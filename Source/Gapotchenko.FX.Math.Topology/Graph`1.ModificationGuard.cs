using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Math.Topology;

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

    readonly struct ModificationGuard
    {
        public ModificationGuard(Graph<TVertex> graph)
        {
            m_Graph = graph;
            m_Version = graph.m_Version;
        }

        readonly Graph<TVertex> m_Graph;
        readonly int m_Version;

        /// <summary>
        /// Throws a graph modification exception.
        /// </summary>
        [DoesNotReturn]
        public static void Throw() =>
            throw new InvalidOperationException("Graph was modified; enumeration operation may not execute.");

        /// <summary>
        /// Ensures that the graph hasn't been modified since the moment the current guard was initialized.
        /// </summary>
        public void Checkpoint()
        {
            if (m_Graph.m_Version != m_Version)
                Throw();
        }

        /// <summary>
        /// Protects <see cref="IEnumerable{T}"/> against graph modifications.
        /// </summary>
        [return: NotNullIfNotNull(nameof(source))]
        public IEnumerable<TVertex>? Protect(IEnumerable<TVertex>? source) => source is null ? null : ProtectCore(source);

        IEnumerable<TVertex> ProtectCore(IEnumerable<TVertex> source)
        {
            foreach (var i in source)
            {
                Checkpoint();
                yield return i;
            }
        }
    }
}
