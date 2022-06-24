using System.Collections;
using System.Diagnostics.CodeAnalysis;

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
    /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty collection; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this ICollection? value) => value is null || value.Count == 0;
}
