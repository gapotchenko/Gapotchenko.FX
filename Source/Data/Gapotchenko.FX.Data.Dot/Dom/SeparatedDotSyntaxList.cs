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
        List<DotSyntaxToken> _separators = new();

        public TNode this[int index] => _nodes[index];

        public int Count => _nodes.Count;

        public IEnumerator<TNode> GetEnumerator() => _nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

        public void Add(TNode node)
        {
            _nodes.Add(node);
            if (_separators.Count + 1 < _nodes.Count)
            {
                _separators.Add(default);
            }
        }

        public void Add(DotSyntaxToken separator)
        {
            _separators.Add(separator);
            if (_nodes.Count < _separators.Count)
            {
                throw new InvalidOperationException("Invalid nodes count.");
            }
        }
    }
}
