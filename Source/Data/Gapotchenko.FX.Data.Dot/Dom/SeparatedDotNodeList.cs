using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a list of nodes with separators.
    /// </summary>
    /// <typeparam name="TNode">Node type.</typeparam>
    /// <typeparam name="TSeparator">Separator token type.</typeparam>
    public class SeparatedDotNodeList<TNode, TSeparator> :
        IReadOnlyList<TNode>,
        IList<TNode>,
        IDotSyntaxSlotProvider
        where TNode : DotNode
        where TSeparator : DotSignificantToken
    {
        readonly List<DotElement> _nodesAndSeparators = new();
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
        /// Gets or sets the node at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the node to get.</param>
        /// <returns>The node at the specified index.</returns>
        public TNode this[int index]
        {
            get => (TNode)_nodesAndSeparators[index * 2];
            set => _nodesAndSeparators[index * 2] = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the number of nodes in the list.
        /// </summary>
        public int Count => (_nodesAndSeparators.Count + 1) / 2;

        /// <summary>
        /// Returns an enumerator that iterates through the nodes.
        /// </summary>
        public IEnumerator<TNode> GetEnumerator() =>
            Enumerable.Range(0, Count)
            .Select(index => this[index])
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Adds the node to the list.
        /// </summary>
        public void Add(TNode node, TSeparator? separator = default)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (_nodesAndSeparators.Count >= 1)
                _nodesAndSeparators.Add(separator ?? _defaultSeparator);

            _nodesAndSeparators.Add(node);

        }

        /// <summary>
        /// Inserts the node at the specified index.
        /// </summary>
        public void Insert(int index, TNode node, TSeparator? separator = default)
        {
            index *= 2;

            if (_nodesAndSeparators.Count != 0)
            {
                if (index == 0)
                    _nodesAndSeparators.Insert(index, separator ?? _defaultSeparator);
                else
                    _nodesAndSeparators.Insert(index - 1, separator ?? _defaultSeparator);
            }

            _nodesAndSeparators.Insert(index, node);
        }

        bool ICollection<TNode>.IsReadOnly => false;

        /// <summary>
        /// Searches for the specified node and returns the zero-based index of the first
        /// occurrence within the entire list.
        /// </summary>
        /// <param name="item">The node to locate in the list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire list,
        /// if found; otherwise, –1.
        /// </returns>
        public int IndexOf(TNode item)
        {
            var index = _nodesAndSeparators.IndexOf(item);
            if (index != -1)
                index /= 2;
            return index;
        }

        void IList<TNode>.Insert(int index, TNode item) => Insert(index, item);

        /// <summary>
        /// Removes the node at the specified index of the list.
        /// </summary>
        /// <param name="index">The zero-based index of the node to remove.</param>
        public void RemoveAt(int index)
        {
            if (index <= 0)
            {
                if (_nodesAndSeparators.Count <= 1)
                    _nodesAndSeparators.RemoveAt(index);
                else
                    _nodesAndSeparators.RemoveRange(index, 2);
            }
            else
            {
                index = index * 2 - 1;
                _nodesAndSeparators.RemoveRange(index, 2);
            }
        }

        void ICollection<TNode>.Add(TNode item) => Add(item);

        /// <summary>
        /// Removes all nodes and separators from the list.
        /// </summary>
        public void Clear()
        {
            _nodesAndSeparators.Clear();
        }

        /// <summary>
        /// Determines whether a node is in the list.
        /// </summary>
        /// <param name="item">The node to locate in the list.</param>
        /// <returns>true if node is found in the list; otherwise, false.</returns>
        public bool Contains(TNode item) => _nodesAndSeparators.Contains(item);

        /// <summary>
        /// Copies all the nodes to a compatible one-dimensional array, starting 
        /// at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the nodes copied from the list.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(TNode[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (Count + arrayIndex > array.Length)
                throw new ArgumentException("The number of nodes in the list is greater than the available space of a destination array.");

            foreach (var node in this)
            {
                array[arrayIndex++] = node;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific node from the list.
        /// </summary>
        /// <param name="item">The node to remove from the list.</param>
        /// <returns>
        /// true if item is successfully removed; otherwise, false. This method also returns
        /// false if item was not found in the list.
        /// </returns>
        public bool Remove(TNode item)
        {
            var index = IndexOf(item);
            if (index is -1)
                return false;
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Gets a read-only list of nodes with separators.
        /// </summary>
        public NodesAndSeparatorsList NodesWithSeparators => new(this);

        int IDotSyntaxSlotProvider.SlotCount => _nodesAndSeparators.Count;

        IDotSyntaxSlotProvider IDotSyntaxSlotProvider.GetSlot(int i) => (IDotSyntaxSlotProvider)_nodesAndSeparators[i];

        /// <summary>
        /// Represents a list of nodes and separators.
        /// </summary>
        public struct NodesAndSeparatorsList : IReadOnlyList<DotElement>
        {
            readonly List<DotElement> _list;

            internal NodesAndSeparatorsList(SeparatedDotNodeList<TNode, TSeparator> parent)
            {
                _list = parent._nodesAndSeparators;
            }

            /// <summary>
            /// Gets or sets a node or a separator at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of a node or a separator to get.</param>
            /// <returns>A node or a separator at the specified index.</returns>
            public DotElement this[int index]
            {
                get => _list[index];

                set
                {
                    if (value is null)
                        throw new ArgumentNullException(nameof(value));

                    var isNode = index % 2 == 0;
                    if (isNode && value is not TNode)
                        throw new ArgumentException("A node expected.", nameof(value));
                    else if (!isNode && value is not TSeparator)
                        throw new ArgumentException("A separator expected.", nameof(value));

                    _list[index] = value;
                }
            }

            /// <summary>
            /// Gets the number of nodes and separators in the list.
            /// </summary>
            public int Count => _list.Count;

            /// <summary>
            /// Returns an enumerator that iterates through the nodes and separators.
            /// </summary>
            public IEnumerator<DotElement> GetEnumerator() => _list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
        }
    }
}
