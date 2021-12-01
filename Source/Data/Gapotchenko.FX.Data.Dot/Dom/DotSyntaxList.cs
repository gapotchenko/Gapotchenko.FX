using System.Collections;
using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotSyntaxList<TNode> :
        IReadOnlyList<TNode>,
        IEnumerable<TNode>,
        IEnumerable,
        IReadOnlyCollection<TNode>
    {
        List<TNode> _nodes = new();

        public TNode this[int index] => _nodes[index];

        public int Count => _nodes.Count;

        public IEnumerator<TNode> GetEnumerator() => _nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

        public void Add(TNode node) => _nodes.Add(node);
    }
}