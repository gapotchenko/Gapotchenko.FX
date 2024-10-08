using System.Collections;

namespace Gapotchenko.FX.Collections;

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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsNullOrEmpty([NotNullWhen(false)] this ICollection? value) => value is null || value.Count == 0;
}
