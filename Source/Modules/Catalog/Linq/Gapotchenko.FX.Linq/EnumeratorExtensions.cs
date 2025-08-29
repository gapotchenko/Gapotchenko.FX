// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

namespace Gapotchenko.FX.Linq;

/// <summary>
/// Provides extension methods for <see cref="IEnumerator{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class EnumeratorExtensions
{
    /// <summary>
    /// Returns an enumeration of remaining elements in the <see cref="IEnumerator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
    /// <param name="enumerator">The enumerator.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that enumerates the remaining elements in the <see cref="IEnumerator{T}"/>.</returns>
    public static IEnumerable<T> Rest<T>(this IEnumerator<T> enumerator)
    {
        ArgumentNullException.ThrowIfNull(enumerator);
        while (enumerator.MoveNext())
            yield return enumerator.Current;
    }
}
