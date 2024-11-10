namespace Gapotchenko.FX.Linq;

/// <summary>
/// Provides extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class EnumerableExtensions
{
    // This class is partial.
    // For more code, please take a look at the neighboring source files.

    /// <summary>
    /// Streams all elements of a sequence, ensuring that each element is retrieved only once, despite enumeration restarts.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">A sequence that contains elements to stream.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> sequence of streamed elements.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> Stream<TSource>(this IEnumerable<TSource> source) =>
        (source ?? throw new ArgumentNullException(nameof(source)))
        .GetEnumerator()
        .Rest();

    /// <summary>
    /// Determines whether the end of a sequence matches the specified value by using the default equality comparer for elements' type.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">An <see cref="IEnumerable{T}"/> value to match.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> sequence matches the end of the <paramref name="source"/> sequence;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool EndsWith<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value) => EndsWith(source, value, null);

    /// <summary>
    /// Determines whether the end of a sequence matches the specified value by using a specified equality comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">An <see cref="IEnumerable{T}"/> value to match.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to use to compare elements.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> sequence matches the end of the <paramref name="source"/> sequence;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool EndsWith<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value, IEqualityComparer<TSource>? comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (ReferenceEquals(source, value))
            return true;

        if (EnumerableEx.TryGetNonEnumeratedCount(value, out var count))
        {
            return source.TakeLast(count).SequenceEqual(value, comparer);
        }
        else
        {
            var valueCollection = value.ReifyCollection();
            return source.TakeLast(valueCollection.Count).SequenceEqual(valueCollection, comparer);
        }
    }
}
