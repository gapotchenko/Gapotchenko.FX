namespace Gapotchenko.FX.Collections;

#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY

/// <summary>
/// Array extensions.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Indicates whether the specified array is <see langword="null"/> or empty.
    /// </summary>
    /// <param name="value">The array to test.</param>
    /// <returns><see langword="true"/> if the <paramref name="value"/> parameter is null or an empty array; otherwise, <see langword="false"/>.</returns>        
#if DEBUG
    [Obsolete("Use 'array?.Length is not > 0' expression instead.")]
#endif
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsNullOrEmpty(
        [NotNullWhen(false)]
#if SOURCE_COMPATIBILITY
        this
#endif
        Array? value) => value is null || value.Length == 0;
}

#endif
