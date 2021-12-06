using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a list of syntax nodes with separators.
    /// </summary>
    /// <typeparam name="TNode">Syntax node type.</typeparam>
    public class SeparatedDotNodeList<TNode> :
        IReadOnlyList<TNode>,
        IEnumerable<TNode>,
        IEnumerable,
        IReadOnlyCollection<TNode>,
        ISyntaxSlotProvider
        where TNode : DotNode
    {
        readonly List<TNode> _nodes = new();
        readonly List<DotNodeOrToken> _nodesAndTokens = new();

        /// <summary>
        /// Gets the node at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the node to get.</param>
        /// <returns>The node at the specified index.</returns>
        public TNode this[int index] => _nodes[index];

        /// <summary>
        /// Gets the number of nodes in the list.
        /// </summary>
        public int Count => _nodes.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        public IEnumerator<TNode> GetEnumerator() => _nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

        /// <summary>
        /// Adds the node to the end of the list.
        /// </summary>
        public void Append(TNode node)
        {
            _nodes.Add(node);
            _nodesAndTokens.Add(node);
        }

        /// <summary>
        /// Adds the separator to the end of the list.
        /// </summary>
        public void Append(DotToken separator)
        {
            _nodesAndTokens.Add(separator);
        }

        /// <summary>
        /// Adds the node to the beginning of the list.
        /// </summary>
        public void Prepend(TNode node)
        {
            _nodes.Insert(0, node);
            _nodesAndTokens.Insert(0, node);
        }

        /// <summary>
        /// Adds the separator to the beginning of the list.
        /// </summary>
        public void Prepend(DotToken separator)
        {
            _nodesAndTokens.Insert(0, separator);
        }

        int ISyntaxSlotProvider.SlotCount =>
            _nodesAndTokens.Count;

        SyntaxSlot ISyntaxSlotProvider.GetSlot(int i) =>
            _nodesAndTokens[i];
    }
}
