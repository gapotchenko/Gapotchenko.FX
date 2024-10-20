// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Utils;
using Gapotchenko.FX.Linq;
using System.Collections;
using System.Diagnostics;

#if NET8_0_OR_GREATER
using static System.ArgumentNullException;
using static System.ArgumentOutOfRangeException;
#else
using static Gapotchenko.FX.Collections.Utils.ThrowPolyfills;
#endif

namespace Gapotchenko.FX.Collections.Generic.Kits;

/// <summary>
/// Provides the base implementation of <see cref="IReadOnlySet{T}"/>.
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public abstract class ReadOnlySetKit<T> : IReadOnlySet<T>
{
    /// <summary>
    /// Gets the <see cref="IEqualityComparer{T}"/> object that is used to determine equality for the values in the set.
    /// </summary>
    public virtual IEqualityComparer<T> Comparer => EqualityComparer<T>.Default;

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
        ThrowIfNull(other);

        if (other == this)
            return false;

        int count = Count;

        if (count == 0)
        {
            if (other.TryGetNonEnumeratedCount(out var otherCount))
                return otherCount > 0;
            else
                return other.Any();
        }

        if (other is ReadOnlySetKit<T> rosb)
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

        var (presenceCount, absence) = CalculateInclusivityOf(other);
        return absence && presenceCount == count;
    }

    /// <inheritdoc/>
    public virtual bool IsProperSupersetOf(IEnumerable<T> other)
    {
        ThrowIfNull(other);

        if (other == this)
            return false;

        int count = Count;
        if (count == 0)
            return false;

        if (other.TryGetNonEnumeratedCount(out var otherCount))
            if (otherCount == 0)
                return true;

        if (other is ReadOnlySetKit<T> rosb)
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

        var (presenceCount, absence) = CalculateInclusivityOf(other, true);
        return !absence && presenceCount < count;
    }

    /// <inheritdoc/>
    public virtual bool IsSubsetOf(IEnumerable<T> other)
    {
        ThrowIfNull(other);

        if (other == this)
            return true;

        int count = Count;
        if (count == 0)
            return true;

        if (other is ReadOnlySetKit<T> rosb)
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

        return CalculateInclusivityOf(other).PresenceCount == count;
    }

    /// <inheritdoc/>
    public virtual bool IsSupersetOf(IEnumerable<T> other)
    {
        ThrowIfNull(other);

        if (other == this)
            return true;

        if (other.TryGetNonEnumeratedCount(out var otherCount))
        {
            if (otherCount == 0)
                return true;
        }

        if (other is ReadOnlySetKit<T> rosb)
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
        ThrowIfNull(other);

        if (Count == 0)
            return false;

        foreach (var i in other)
            if (Contains(i))
                return true;

        return false;
    }

    /// <inheritdoc/>
    public virtual bool SetEquals(IEnumerable<T> other)
    {
        ThrowIfNull(other);

        if (other == this)
            return true;

        int expectedCount = Count;

        if (other.TryGetNonEnumeratedCount(out var count))
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

    (int PresenceCount, bool Absence) CalculateInclusivityOf(IEnumerable<T> other, bool stopOnAbsence = false)
    {
        int count = Count;
        if (count == 0)
            return (0, other.Any());

        var presence = new HashSet<T>(Comparer);
        bool absence = false;

        foreach (var i in other)
        {
            if (Contains(i))
            {
                presence.Add(i);
            }
            else
            {
                absence = true;
                if (stopOnAbsence)
                    break;
            }
        }

        return (presence.Count, absence);
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
        ThrowIfNull(array);
        ThrowIfNegative(arrayIndex);
        ThrowIfNegative(count);
        if (count > array.Length - arrayIndex)
            ExceptionHelper.ThrowArgumentException_ArrayPlusOffTooSmall();

        if (count == 0)
            return;

        foreach (var i in this)
        {
            array[arrayIndex++] = i;
            if (--count == 0)
                break;
        }
    }
}
