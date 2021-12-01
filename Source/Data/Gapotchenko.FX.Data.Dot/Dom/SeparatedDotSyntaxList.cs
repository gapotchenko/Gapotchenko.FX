using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class SeparatedDotSyntaxList<TNode> :
        IReadOnlyList<TNode>,
        IEnumerable<TNode>,
        IEnumerable,
        IReadOnlyCollection<TNode>
        where TNode : DotSyntaxNode
    {
        List<TNode> _nodes = new();
        List<DotSyntaxNodeOrToken> _nodesAndTokens = new();

        public TNode this[int index] => _nodes[index];

        public int Count => _nodes.Count;

        public IEnumerator<TNode> GetEnumerator() => _nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

        public void Append(TNode node)
        {
            _nodes.Add(node);
            _nodesAndTokens.Add(node);
        }

        public void Append(DotSyntaxToken separator)
        {
            _nodesAndTokens.Add(separator);
        }

        public void Prepend(TNode node)
        {
            _nodes.Insert(0, node);
            _nodesAndTokens.Insert(0, node);
        }

        public void Prepend(DotSyntaxToken separator)
        {
            _nodesAndTokens.Insert(0, separator);
        }
    }
}
