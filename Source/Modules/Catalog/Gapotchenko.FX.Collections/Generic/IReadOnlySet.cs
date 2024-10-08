// (c) Portions of code from the .NET project by Microsoft and .NET Foundation

#if !TFF_IREADONLYSET

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Collections.Generic;

/// <summary>
/// <para>
/// Provides a read-only abstraction of a set.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
/// <typeparam name="T">The type of elements in the set.</typeparam>
public interface IReadOnlySet<T> : IReadOnlyCollection<T>
{
    /// <summary>
    /// Determines if the set contains a specific item
    /// </summary>
    /// <param name="item">The item to check if the set contains.</param>
    /// <returns><see langword="true" /> if found; otherwise <see langword="false" />.</returns>
    bool Contains(T item);

    /// <summary>
    /// Determines whether the current set is a proper (strict) subset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true" /> if the current set is a proper subset of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
    bool IsProperSubsetOf(IEnumerable<T> other);

    /// <summary>
    /// Determines whether the current set is a proper (strict) superset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true" /> if the collection is a proper superset of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
    bool IsProperSupersetOf(IEnumerable<T> other);

    /// <summary>
    /// Determines whether the current set is a subset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true" /> if the current set is a subset of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
    bool IsSubsetOf(IEnumerable<T> other);

    /// <summary>
    /// Determines whether the current set is a superset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true" /> if the current set is a superset of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
    bool IsSupersetOf(IEnumerable<T> other);

    /// <summary>
    /// Determines whether the current set overlaps with the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true" /> if the current set and other share at least one common element; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
    bool Overlaps(IEnumerable<T> other);

    /// <summary>
    /// Determines whether the current set and the specified collection contain the same elements.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true" /> if the current set is equal to other; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
    bool SetEquals(IEnumerable<T> other);
}

#else

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(IReadOnlySet<>))]

#endif
