using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a list of syntax nodes.
    /// </summary>
    /// <typeparam name="TNode">Syntax node type.</typeparam>
    public class DotNodeList<TNode> :
        IReadOnlyList<TNode>,
        IEnumerable<TNode>,
        IEnumerable,
        IReadOnlyCollection<TNode>
        where TNode : DotNode
    {
        readonly LinkedList<TNode> _nodes = new();

        /// <summary>
        /// Gets the node at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the node to get.</param>
        /// <returns>The node at the specified index.</returns>
        public TNode this[int index] => _nodes.ElementAt(index);

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
        public void Append(TNode node) => _nodes.AddLast(node);

        /// <summary>
        /// Adds the node to the beginning of the list.
        /// </summary>
        public void Prepend(TNode node) => _nodes.AddFirst(node);
    }
}