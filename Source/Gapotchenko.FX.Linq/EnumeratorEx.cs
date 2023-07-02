namespace Gapotchenko.FX.Linq;

/// <summary>
/// Provides an extended set of static methods for querying objects that implement <see cref="IEnumerator{T}"/>.
/// </summary>
public static class EnumeratorEx
{
    /// <summary>
    /// Returns an enumeration of remaining elements in the <see cref="IEnumerator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
    /// <param name="enumerator">The enumerator.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that provides the remaining elements in the <see cref="IEnumerator{T}"/>.</returns>
    public static IEnumerable<T> Rest<T>(this IEnumerator<T> enumerator)
    {
        if (enumerator == null)
            throw new ArgumentNullException(nameof(enumerator));
        while (enumerator.MoveNext())
            yield return enumerator.Current;
    }
}
