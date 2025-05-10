namespace Gapotchenko.FX.Collections.Generic;

#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY

/// <summary>
/// Generic read-only collection extensions.
/// </summary>
public static class ReadOnlyCollectionExtensions
{
    /// <summary>
    /// Indicates whether the specified collection is null or empty.
    /// </summary>
    /// <param name="value">The collection to test.</param>
    /// <returns><see langword="true"/> if the <paramref name="value"/> parameter is null or an empty collection; otherwise, <see langword="false"/>.</returns>
    [Obsolete("Use 'collection?.Length is not > 0' expression instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsNullOrEmpty<T>(
        [NotNullWhen(false)]
#if SOURCE_COMPATIBILITY
        this
#endif
        IReadOnlyCollection<T>? value) => value is null || value.Count == 0;
}

#endif
