using System.Collections;

namespace Gapotchenko.FX.Collections;

#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY

/// <summary>
/// Dictionary extensions.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Indicates whether the specified dictionary is null or empty.
    /// </summary>
    /// <param name="value">The dictionary to test.</param>
    /// <returns><see langword="true"/> if the <paramref name="value"/> parameter is null or an empty dictionary; otherwise, <see langword="false"/>.</returns>
    [Obsolete("Use 'dictionary?.Length is not > 0' expression instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsNullOrEmpty(
        [NotNullWhen(false)]
#if SOURCE_COMPATIBILITY
        this
#endif
        IDictionary? value) => value is null || value.Count == 0;
}

#endif
