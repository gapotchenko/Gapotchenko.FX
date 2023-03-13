using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Collections;

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
    public static bool IsNullOrEmpty([NotNullWhen(false)] this IDictionary? value) => value is null || value.Count == 0;
}
