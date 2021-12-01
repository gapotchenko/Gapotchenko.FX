using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotSyntaxList<TNode> :
        IReadOnlyList<TNode>,
        IEnumerable<TNode>,
        IEnumerable,
        IReadOnlyCollection<TNode>
    {
        LinkedList<TNode> _nodes = new();

        public TNode this[int index] => _nodes.ElementAt(index);

        public int Count => _nodes.Count;

        public IEnumerator<TNode> GetEnumerator() => _nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

        public void Append(TNode node) => _nodes.AddLast(node);
        public void Prepend(TNode node) => _nodes.AddFirst(node);
    }
}