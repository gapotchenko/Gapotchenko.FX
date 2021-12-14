using System;
using System.Collections;
using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a list of syntax nodes with separators.
    /// </summary>
    /// <typeparam name="TNode">Syntax node type.</typeparam>
    /// <typeparam name="TSeparator">Separator token type.</typeparam>
    public class SeparatedDotNodeList<TNode, TSeparator> :
        IReadOnlyList<TNode>,
        IEnumerable<TNode>,
        IEnumerable,
        IReadOnlyCollection<TNode>,
        IDotSyntaxSlotProvider
        where TNode : DotNode
        where TSeparator : DotSignificantToken
    {
        readonly List<TNode> _nodes = new();
        readonly List<IDotSyntaxSlotProvider> _nodesAndTokens = new();
        readonly TSeparator _defaultSeparator;

        /// <summary>
        /// Creates a new <see cref="SeparatedDotNodeList{TNode, TSeparator}"/> instance.
        /// </summary>
        public SeparatedDotNodeList(TSeparator separator)
        {
            if (separator is null)
                throw new ArgumentNullException(nameof(separator));

            _defaultSeparator = separator;
        }

        /// <summary>
        /// Creates a new <see cref="SeparatedDotNodeList{TNode, TSeparator}"/> instance.
        /// </summary>
        public SeparatedDotNodeList(IEnumerable<TNode> nodes, TSeparator separator)
        {
            if (nodes is null)
                throw new ArgumentNullException(nameof(nodes));
            if (separator is null)
                throw new ArgumentNullException(nameof(separator));

            _defaultSeparator = separator;

            foreach (var item in nodes)
            {
                Add(item, separator);
            }
        }

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
        /// Returns an enumerator that iterates through the nodes.
        /// </summary>
        public IEnumerator<TNode> GetEnumerator() => _nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

        /// <summary>
        /// Adds the node to the end of the list.
        /// </summary>
        public void Add(TNode node, TSeparator? separator = default)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            _nodes.Add(node);

            if (_nodes.Count > 1)
                _nodesAndTokens.Add(separator ?? _defaultSeparator);

            _nodesAndTokens.Add(node);

        }

        /// <summary>
        /// Adds the node to the beginning of the list.
        /// </summary>
        public void AddFirst(TNode node, TSeparator? separator = default)
        {
            _nodes.Insert(0, node);

            if (_nodes.Count > 1)
                _nodesAndTokens.Insert(0, separator ?? _defaultSeparator);

            _nodesAndTokens.Insert(0, node);
        }

        int IDotSyntaxSlotProvider.SlotCount =>
            _nodesAndTokens.Count;

        IDotSyntaxSlotProvider IDotSyntaxSlotProvider.GetSlot(int i) =>
            _nodesAndTokens[i];
    }
}
