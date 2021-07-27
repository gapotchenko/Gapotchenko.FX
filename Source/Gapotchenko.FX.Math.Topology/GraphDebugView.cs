using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    [DebuggerDisplay("Count = {Count}")]
    sealed class CountedEnumerableView<T>
    {
        public CountedEnumerableView(IEnumerable<T> source)
        {
            _Source = source;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEnumerable<T> _Source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _Source.ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Count => _Source.Count();
    }

    sealed class GraphDebugView<T> where T : notnull
    {
        public GraphDebugView(Graph<T> graph)
        {
            _Graph = graph;
        }

        readonly Graph<T> _Graph;

        public object Vertices => new CountedEnumerableView<T>(_Graph.Vertices);

        public object Edges => new CountedEnumerableView<(T A, T B)>(_Graph.Edges);

        public object AdjacencyList => _Graph.AdjacencyList;
    }
}
