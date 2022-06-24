using System;
using System.Collections.Generic;

namespace Gapotchenko.FX;

/// <summary>
/// Provides extended functionality for building, combining and diffusing hash codes.
/// </summary>
public static class HashCodeEx
{
    /// <summary>
    /// Combines hash codes of the elements of a specified sequence.
    /// </summary>
    /// <typeparam name="T">The sequence element type.</typeparam>
    /// <param name="source">The sequence of elements. The sequence itself can be <c>null</c>, and it can contain elements that are <c>null</c>.</param>
    /// <returns>The combined hash code.</returns>
    public static int SequenceCombine<T>(IEnumerable<T>? source)
    {
        var hc = new HashCode();
        hc.AddRange(source);
        return hc.ToHashCode();
    }

    /// <summary>
    /// Combines hash codes of the elements of a specified sequence by using a specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="T">The sequence element type.</typeparam>
    /// <param name="source">The sequence of elements. The sequence itself can be <c>null</c>, and it can contain elements that are <c>null</c>.</param>
    /// <param name="comparer">The equality comparer to get element hash codes from.</param>
    /// <returns>The combined hash code.</returns>
    public static int SequenceCombine<T>(IEnumerable<T>? source, IEqualityComparer<T>? comparer)
    {
        var hc = new HashCode();
        hc.AddRange(source, comparer);
        return hc.ToHashCode();
    }

    /// <summary>
    /// Adds hash codes of the elements of a specified sequence to this instance.
    /// </summary>
    /// <typeparam name="T">The sequence element type.</typeparam>
    /// <param name="hashCode">The hash code.</param>
    /// <param name="source">The sequence of elements. The sequence itself can be <c>null</c>, and it can contain elements that are <c>null</c>.</param>
    /// <returns>The combined hash code.</returns>
    public static void AddRange<T>(this ref HashCode hashCode, IEnumerable<T>? source)
    {
        if (source != null)
            foreach (var i in source)
                hashCode.Add(i);
    }

    /// <summary>
    /// Adds hash codes of the elements of a specified sequence to this instance by using a specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="T">The sequence element type.</typeparam>
    /// <param name="hashCode">The hash code.</param>
    /// <param name="source">The sequence of elements. The sequence itself can be <c>null</c>, and it can contain elements that are <c>null</c>.</param>
    /// <param name="comparer">The equality comparer to get element hash codes from.</param>
    /// <returns>The combined hash code.</returns>
    public static void AddRange<T>(this ref HashCode hashCode, IEnumerable<T>? source, IEqualityComparer<T>? comparer)
    {
        if (source != null)
            foreach (var i in source)
                hashCode.Add(i, comparer);
    }
}
