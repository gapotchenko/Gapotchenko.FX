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
    /// Streams all elements of a sequence by ensuring that every element is retrieved only once.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">A sequence that contains elements to stream.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> sequence of streamed elements.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> Stream<TSource>(this IEnumerable<TSource> source) =>
        (source ?? throw new ArgumentNullException(nameof(source)))
        .GetEnumerator()
        .Rest();
}
