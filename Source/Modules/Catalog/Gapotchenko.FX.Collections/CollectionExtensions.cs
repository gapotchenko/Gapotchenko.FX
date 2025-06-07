using System.Collections;

namespace Gapotchenko.FX.Collections;

#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY

/// <summary>
/// Collection extensions.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Indicates whether the specified collection is null or empty.
    /// </summary>
    /// <param name="value">The collection to test.</param>
    /// <returns><see langword="true"/> if the <paramref name="value"/> parameter is null or an empty collection; otherwise, <see langword="false"/>.</returns>
    [Obsolete("Use 'collection is null or []' expression instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsNullOrEmpty(
        [NotNullWhen(false)]
#if SOURCE_COMPATIBILITY
        this
#endif
        ICollection? value) => value is null || value.Count == 0;
}

#endif
