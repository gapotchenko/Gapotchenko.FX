// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.InteropServices;

namespace Gapotchenko.FX;

/// <summary>
/// Provides information about environment variables on the current platform.
/// </summary>
public static class EnvironmentVariables
{
    /// <summary>
    /// Gets a platform-specific environment variable name comparison.
    /// </summary>
    public static StringComparison NameComparison =>
        NamesAreCaseSensitive ?
            StringComparison.Ordinal :
            StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Gets a platform-specific environment variable name comparer.
    /// </summary>
    public static StringComparer NameComparer =>
        NamesAreCaseSensitive ?
            StringComparer.Ordinal :
            StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Gets a value indicating whether environment variable names under the current operating system are case-sensitive.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if environment variable names are case-sensitive; otherwise, <see langword="false"/>.
    /// </value>
    public static bool NamesAreCaseSensitive { get; } = NamesAreCaseSensitiveCore();

    static bool NamesAreCaseSensitiveCore()
    {
        return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
}
