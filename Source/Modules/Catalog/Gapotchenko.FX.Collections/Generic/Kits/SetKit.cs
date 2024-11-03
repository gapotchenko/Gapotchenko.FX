// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Linq;
using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Generic.Kits;

/// <summary>
/// Provides a base implementation of <see cref="ISet{T}"/>.
/// </summary>
public abstract class SetKit<T> : ReadOnlySetKit<T>, ISet<T>
{
    /// <inheritdoc/>
    public abstract bool Add(T item);

    void ICollection<T>.Add(T item) => Add(item);

    /// <summary>
    /// Removes the specified element from the current set.
    /// </summary>
    /// <param name="item">The element to remove.</param>
    /// <returns>
    /// <see langword="true"/> if the element is successfully found and removed;
    /// otherwise, <see langword="false"/>.
    /// This method returns <see langword="false"/> if item is not found in the set.
    /// </returns>
    public abstract bool Remove(T item);

    /// <summary>
    /// Removes all elements from the current set.
    /// </summary>
    public abstract void Clear();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;

    /// <summary>
    /// Ensures that the current set is not read-only.
    /// </summary>
    /// <exception cref="InvalidOperationException">The set is read-only.</exception>
    protected void EnsureNotReadOnly()
    {
        bool isReadOnly = ((ICollection<T>)this).IsReadOnly;
        if (isReadOnly)
            throw new InvalidOperationException("The set is read-only.");
    }

    /// <inheritdoc/>
    public virtual void ExceptWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        EnsureNotReadOnly();

        if (Count == 0)
        {
            // Already empty.
            return;
        }

        if (other == this)
        {
            Clear();
        }
        else
        {
            foreach (var i in other)
                Remove(i);
        }
    }

    /// <inheritdoc/>
    public virtual void IntersectWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        EnsureNotReadOnly();

        if (other == this)
        {
            // Intersection with itself keeps the set intact.
            return;
        }

        if (Count == 0)
        {
            // Already empty.
            return;
        }

        if (other.TryGetNonEnumeratedCount(out var otherCount))
        {
            if (otherCount == 0)
            {
                // Intersection with an empty set produces an empty set.
                Clear();
                return;
            }
        }

        if (other is ReadOnlySetKit<T> rosb)
        {
            if (rosb.Comparer.Equals(Comparer))
            {
                IntersectWithSetWithCompatibleComparer(rosb);
                return;
            }
        }
        else if (other is HashSet<T> hs)
        {
            if (hs.Comparer.Equals(Comparer))
            {
                IntersectWithSetWithCompatibleComparer(hs);
                return;
            }
        }

        IntersectWithEnumerable(other);
    }

    void IntersectWithSetWithCompatibleComparer(IReadOnlySet<T> other)
    {
        var itemsToRemove = new List<T>();

        foreach (var i in this)
            if (!other.Contains(i))
                itemsToRemove.Add(i);

        ExceptWith(itemsToRemove);
    }

#if !NET5_0_OR_GREATER
    void IntersectWithSetWithCompatibleComparer(HashSet<T> other)
    {
        var itemsToRemove = new List<T>();

        foreach (var i in this)
            if (!other.Contains(i))
                itemsToRemove.Add(i);

        ExceptWith(itemsToRemove);
    }
#endif

    void IntersectWithEnumerable(IEnumerable<T> other)
    {
        var itemsToKeep = new HashSet<T>(Comparer);

        foreach (var i in other)
            if (Contains(i))
                itemsToKeep.Add(i);

        IntersectWithSetWithCompatibleComparer(itemsToKeep);
    }

    /// <inheritdoc/>
    public virtual void SymmetricExceptWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        EnsureNotReadOnly();

        if (Count == 0)
        {
            UnionWith(other);
            return;
        }

        if (other == this)
        {
            // Symmetric except with itself empties the set.
            Clear();
            return;
        }

        if (other is ReadOnlySetKit<T> rosb)
        {
            if (rosb.Comparer.Equals(Comparer))
            {
                SymmetricExceptWithSetWithCompatibleComparer(rosb);
                return;
            }
        }
        else if (other is HashSet<T> hs)
        {
            if (hs.Comparer.Equals(Comparer))
            {
                SymmetricExceptWithSetWithCompatibleComparer(hs);
                return;
            }
        }

        SymmetricExceptWithEnumerable(other);
    }

    void SymmetricExceptWithSetWithCompatibleComparer(IEnumerable<T> other)
    {
        foreach (var i in other)
            if (!Remove(i))
                Add(i);
    }

    void SymmetricExceptWithEnumerable(IEnumerable<T> other) => SymmetricExceptWithSetWithCompatibleComparer(other.Distinct(Comparer));

    /// <inheritdoc/>
    public virtual void UnionWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        EnsureNotReadOnly();

        if (other == this)
            return;

        foreach (var i in other)
            Add(i);
    }
}
