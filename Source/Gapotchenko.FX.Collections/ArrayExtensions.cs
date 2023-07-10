using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Collections;

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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsNullOrEmpty([NotNullWhen(false)] this Array? value) => value is null || value.Length == 0;
}
