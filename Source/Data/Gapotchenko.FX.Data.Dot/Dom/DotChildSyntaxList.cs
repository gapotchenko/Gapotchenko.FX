using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents syntax node children list.
    /// </summary>
    public struct DotChildSyntaxList : IReadOnlyList<DotSyntaxNodeOrToken>
    {
        readonly DotSyntaxNode? _node;
        readonly int _count;

        internal DotChildSyntaxList(DotSyntaxNode node)
        {
            _node = node;
            _count = CountNodes(node);
        }

        /// <summary>
        /// Gets the number of children contained in the <see cref="DotChildSyntaxList"/>.
        /// </summary>
        public int Count => _count;

        internal static int CountNodes(DotSyntaxNode node)
        {
            int n = 0;

            for (int i = 0, s = node.SlotCount; i < s; i++)
            {
                var child = node.GetSlot(i);
                if (!child.IsDefault)
                {
                    if (!child.IsList)
                    {
                        n++;
                    }
                    else
                    {
                        n += child.SlotCount;
                    }
                }
            }

            return n;
        }

        /// <summary>Gets the child at the specified index.</summary>
        /// <param name="index">The zero-based index of the child to get.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is less than 0.-or-<paramref name="index" /> is equal to or greater than <see cref="DotChildSyntaxList.Count"/>. </exception>
        public DotSyntaxNodeOrToken this[int index]
        {
            get
            {
                if (unchecked((uint)index < (uint)_count))
                {
                    return ItemInternal(_node!, index);
                }

                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal DotSyntaxNode? Node => _node;

        static int Occupancy(SyntaxSlot slot)
        {
            return slot.IsList ? slot.SlotCount : 1;
        }

        /// <summary>
        /// Internal indexer that does not verify index.
        /// Used when caller has already ensured that index is within bounds.
        /// </summary>
        internal static DotSyntaxNodeOrToken ItemInternal(DotSyntaxNode node, int index)
        {
            SyntaxSlot childSlot;
            var idx = index;
            var slotIndex = 0;

            // Find a slot that contains the node or its parent list (if node is in a list).
            //
            // At the end of this loop we will have
            // 1) slot index - slotIdx
            // 2) if the slot is a list, node index in the list - idx
            while (true)
            {
                childSlot = node.GetSlot(slotIndex);
                if (!childSlot.IsDefault)
                {
                    int currentOccupancy = Occupancy(childSlot);
                    if (idx < currentOccupancy)
                    {
                        break;
                    }

                    idx -= currentOccupancy;
                }

                slotIndex++;
            }

            if (!childSlot.IsList)
            {
                if (childSlot.IsToken)
                    return childSlot.AsToken()!;
                else if (childSlot.IsNode)
                    return childSlot.AsNode()!;
                else
                    throw new InvalidOperationException();
            }
            else
            {
                return Unwrap(childSlot.GetSlot(idx));
            }
        }

        static DotSyntaxNodeOrToken Unwrap(SyntaxSlot slot)
        {
            if (slot.IsToken)
                return slot.AsToken()!;
            else if (slot.IsNode)
                return slot.AsNode()!;
            else
                throw new ArgumentException("Cannot unwrap the given slot.", nameof(slot));
        }

        /// <summary>
        /// Determines whether any child exists.
        /// </summary>
        public bool Any()
        {
            return _count != 0;
        }

        /// <summary>
        /// Returns the first child in the list.
        /// </summary>
        /// <returns>The first child in the list.</returns>
        /// <exception cref="System.InvalidOperationException">The list is empty.</exception>    
        public DotSyntaxNodeOrToken First()
        {
            if (Any())
            {
                return this[0];
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns the last child in the list.
        /// </summary>
        /// <returns>The last child in the list.</returns>
        /// <exception cref="System.InvalidOperationException">The list is empty.</exception>    
        public DotSyntaxNodeOrToken Last()
        {
            if (Any())
            {
                return this[_count - 1];
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns a list which contains all children of <see cref="DotChildSyntaxList"/> in reversed order.
        /// </summary>
        /// <returns><see cref="Reversed"/> which contains all children of <see cref="DotChildSyntaxList"/> in reversed order</returns>
        public Reversed Reverse()
        {
            Debug.Assert(_node is object);
            return new Reversed(_node, _count);
        }

        /// <summary>Returns an enumerator that iterates through the <see cref="DotChildSyntaxList"/>.</summary>
        /// <returns>A <see cref="Enumerator"/> for the <see cref="DotChildSyntaxList"/>.</returns>
        public Enumerator GetEnumerator()
        {
            if (_node == null)
            {
                return default;
            }

            return new Enumerator(_node, _count);
        }

        IEnumerator<DotSyntaxNodeOrToken> IEnumerable<DotSyntaxNodeOrToken>.GetEnumerator()
        {
            if (_node == null)
            {
                return Enumerable.Empty<DotSyntaxNodeOrToken>().GetEnumerator();
            }

            return new EnumeratorImpl(_node, _count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_node == null)
            {
                return Enumerable.Empty<DotSyntaxNodeOrToken>().GetEnumerator();
            }

            return new EnumeratorImpl(_node, _count);
        }

        /// <summary>Enumerates the elements of a <see cref="DotChildSyntaxList" />.</summary>
        public struct Enumerator
        {
            DotSyntaxNode? _node;
            int _count;
            int _childIndex;

            internal Enumerator(DotSyntaxNode node, int count)
            {
                _node = node;
                _count = count;
                _childIndex = -1;
            }

            // PERF: Initialize an Enumerator directly from a DotSyntaxNode without going
            // via ChildNodesAndTokens. This saves constructing an intermediate DotChildSyntaxList
            internal void InitializeFrom(DotSyntaxNode node)
            {
                _node = node;
                _count = CountNodes(node);
                _childIndex = -1;
            }

            /// <summary>Advances the enumerator to the next element of the <see cref="DotChildSyntaxList" />.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                var newIndex = _childIndex + 1;
                if (newIndex < _count)
                {
                    _childIndex = newIndex;
                    Debug.Assert(_node != null);
                    return true;
                }

                return false;
            }

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            /// <returns>The element in the <see cref="DotChildSyntaxList" /> at the current position of the enumerator.</returns>
            public DotSyntaxNodeOrToken Current
            {
                get
                {
                    Debug.Assert(_node is object);
                    return ItemInternal(_node, _childIndex);
                }
            }

            /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
            public void Reset()
            {
                _childIndex = -1;
            }

            internal bool TryMoveNextAndGetCurrent(out DotSyntaxNodeOrToken current)
            {
                if (!MoveNext())
                {
                    current = default;
                    return false;
                }

                current = ItemInternal(_node!, _childIndex);
                return true;
            }
        }

        sealed class EnumeratorImpl : IEnumerator<DotSyntaxNodeOrToken>
        {
            Enumerator _enumerator;

            internal EnumeratorImpl(DotSyntaxNode node, int count)
            {
                _enumerator = new Enumerator(node, count);
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            ///   </returns>
            public DotSyntaxNodeOrToken Current
            {
                get { return _enumerator.Current; }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            ///   </returns>
            object IEnumerator.Current
            {
                get { return _enumerator.Current; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                _enumerator.Reset();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            { }
        }

        /// <summary>
        /// Represents reversed syntax node children list.
        /// </summary>
        public readonly partial struct Reversed : IEnumerable<DotSyntaxNodeOrToken>
        {
            readonly DotSyntaxNode? _node;
            readonly int _count;

            internal Reversed(DotSyntaxNode node, int count)
            {
                _node = node;
                _count = count;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            public Enumerator GetEnumerator()
            {
                Debug.Assert(_node is object);
                return new Enumerator(_node, _count);
            }

            IEnumerator<DotSyntaxNodeOrToken> IEnumerable<DotSyntaxNodeOrToken>.GetEnumerator()
            {
                if (_node == null)
                {
                    return Enumerable.Empty<DotSyntaxNodeOrToken>().GetEnumerator();
                }

                return new EnumeratorImpl(_node, _count);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                if (_node == null)
                {
                    return Enumerable.Empty<DotSyntaxNodeOrToken>().GetEnumerator();
                }

                return new EnumeratorImpl(_node, _count);
            }

            /// <summary>
            /// A reversed collection enumerator.
            /// </summary>
            public struct Enumerator
            {
                readonly DotSyntaxNode? _node;
                readonly int _count;
                int _childIndex;

                internal Enumerator(DotSyntaxNode node, int count)
                {
                    _node = node;
                    _count = count;
                    _childIndex = count;
                }

                /// <summary>
                /// Advances the enumerator to the next element of the collection.
                /// </summary>
                [MemberNotNullWhen(true, nameof(_node))]
                public bool MoveNext()
                {
                    return --_childIndex >= 0;
                }

                /// <summary>
                /// Gets the element in the collection at the current position of the enumerator.
                /// </summary>
                public DotSyntaxNodeOrToken Current
                {
                    get
                    {
                        Debug.Assert(_node is object);
                        return ItemInternal(_node, _childIndex);
                    }
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                public void Reset()
                {
                    _childIndex = _count;
                }
            }

            sealed class EnumeratorImpl : IEnumerator<DotSyntaxNodeOrToken>
            {
                readonly Enumerator _enumerator;

                internal EnumeratorImpl(DotSyntaxNode node, int count)
                {
                    _enumerator = new Enumerator(node, count);
                }

                /// <summary>
                /// Gets the element in the collection at the current position of the enumerator.
                /// </summary>
                /// <returns>
                /// The element in the collection at the current position of the enumerator.
                ///   </returns>
                public DotSyntaxNodeOrToken Current => _enumerator.Current;

                /// <summary>
                /// Gets the element in the collection at the current position of the enumerator.
                /// </summary>
                /// <returns>
                /// The element in the collection at the current position of the enumerator.
                ///   </returns>
                object IEnumerator.Current => _enumerator.Current;

                /// <summary>
                /// Advances the enumerator to the next element of the collection.
                /// </summary>
                /// <returns>
                /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
                /// </returns>
                /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created. </exception>
                public bool MoveNext() => _enumerator.MoveNext();

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created. </exception>
                public void Reset() => _enumerator.Reset();

                /// <summary>
                /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose() { }
            }
        }
    }
}