using Gapotchenko.FX.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Collections.Generic.Foundry
{
    /// <summary>
    /// Provides the implementation base for <see cref="IReadOnlySet{T}"/>.
    /// </summary>
    public abstract class ReadOnlySetBase<T> : IReadOnlySet<T>
    {
        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> object that is used to determine equality for the values in the set.
        /// </summary>
        public abstract IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// Gets the number of elements that are contained in a set.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Determines whether the current set contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate in the set.</param>
        /// <returns>
        /// <see langword="true"/> if the set contains the specified element;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public abstract bool Contains(T item);

        /// <summary>
        /// Returns an enumerator that iterates through the set.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> object for the set.</returns>
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public virtual bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            int count = Count;

            if (count == 0)
            {
                if (EnumerableEx.TryGetNonEnumeratedCount(other, out var otherCount))
                    return otherCount > 0;
                else
                    return other.Any();
            }

            if (other is ReadOnlySetBase<T> rosb)
            {
                if (rosb.Comparer.Equals(Comparer))
                {
                    if (count >= rosb.Count)
                        return false;
                    foreach (var i in this)
                    {
                        if (!rosb.Contains(i))
                            return false;
                    }
                    return true;
                }
            }
            else if (other is HashSet<T> hs)
            {
                if (hs.Comparer.Equals(Comparer))
                {
                    if (count >= hs.Count)
                        return false;
                    foreach (var i in this)
                    {
                        if (!hs.Contains(i))
                            return false;
                    }
                    return true;
                }
            }

            var inclusivity = CalculateInclusivityOf(other);
            return inclusivity.PresentCount == count && inclusivity.HasAbsent;
        }

        /// <inheritdoc/>
        public virtual bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            int count = Count;
            if (count == 0)
                return false;

            if (EnumerableEx.TryGetNonEnumeratedCount(other, out var otherCount))
                if (otherCount == 0)
                    return true;

            if (other is ReadOnlySetBase<T> rosb)
            {
                if (rosb.Comparer.Equals(Comparer))
                {
                    if (rosb.Count >= count)
                        return false;

                    return ContainsAllElements(rosb);
                }
            }
            else if (other is HashSet<T> hs)
            {
                if (hs.Comparer.Equals(Comparer))
                {
                    if (hs.Count >= count)
                        return false;

                    return ContainsAllElements(hs);
                }
            }

            var inclusivity = CalculateInclusivityOf(other, true);
            return inclusivity.PresentCount < count && !inclusivity.HasAbsent;
        }

        /// <inheritdoc/>
        public virtual bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            int count = Count;
            if (count == 0)
                return true;

            if (other is ReadOnlySetBase<T> rosb)
            {
                if (rosb.Comparer.Equals(Comparer))
                {
                    if (count > rosb.Count)
                        return false;
                    foreach (var i in this)
                    {
                        if (!rosb.Contains(i))
                            return false;
                    }
                    return true;
                }
            }
            else if (other is HashSet<T> hs)
            {
                if (hs.Comparer.Equals(Comparer))
                {
                    if (count > hs.Count)
                        return false;
                    foreach (var i in this)
                    {
                        if (!hs.Contains(i))
                            return false;
                    }
                    return true;
                }
            }

            return CalculateInclusivityOf(other).PresentCount == count;
        }

        /// <inheritdoc/>
        public virtual bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (EnumerableEx.TryGetNonEnumeratedCount(other, out var otherCount))
            {
                if (otherCount == 0)
                    return true;
            }

            if (other is ReadOnlySetBase<T> rosb)
            {
                if (rosb.Comparer.Equals(Comparer))
                {
                    if (rosb.Count > Count)
                        return false;
                }
            }
            else if (other is HashSet<T> hs)
            {
                if (hs.Comparer.Equals(Comparer))
                {
                    if (hs.Count > Count)
                        return false;
                }
            }

            return ContainsAllElements(other);
        }

        /// <inheritdoc/>
        public virtual bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            foreach (var i in other)
                if (Contains(i))
                    return true;

            return false;
        }

        /// <inheritdoc/>
        public virtual bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            int expectedCount = Count;

            if (EnumerableEx.TryGetNonEnumeratedCount(other, out var count))
            {
                return count == expectedCount && ContainsAllElements(other);
            }
            else
            {
                foreach (var i in other)
                {
                    ++count;
                    if (count > expectedCount)
                        return false;

                    if (!Contains(i))
                        return false;
                }

                if (count != expectedCount)
                    return false;

                return true;
            }
        }

        (int PresentCount, bool HasAbsent) CalculateInclusivityOf(IEnumerable<T> other, bool stopOnAbsent = false)
        {
            int count = Count;
            if (count == 0)
                return (0, other.Any());

            var present = new HashSet<T>(Comparer);
            bool hasAbsent = false;

            foreach (var i in other)
            {
                if (Contains(i))
                {
                    present.Add(i);
                }
                else
                {
                    hasAbsent = true;
                    if (stopOnAbsent)
                        break;
                }
            }

            return (present.Count, hasAbsent);
        }

        bool ContainsAllElements(IEnumerable<T> other)
        {
            foreach (var i in other)
                if (!Contains(i))
                    return false;

            return true;
        }

        /// <summary>
        /// Copies the elements of the current set to an array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the current set.
        /// The array must have zero-based indexing.
        /// </param>
        public void CopyTo(T[] array) => CopyTo(array, 0);

        /// <summary>
        /// Copies the elements of the current set to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the current set.
        /// The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) => CopyTo(array, arrayIndex, Count);

        /// <summary>
        /// Copies the specified number of elements of the current set to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the current set.
        /// The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <param name="count">The number of elements to copy to array.</param>
        public virtual void CopyTo(T[] array, int arrayIndex, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number required.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
            if (count > array.Length - arrayIndex)
                throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            foreach (var i in this)
                array[arrayIndex++] = i;
        }
    }
}
