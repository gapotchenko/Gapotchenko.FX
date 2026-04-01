// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Diagnostics;

#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY

partial class CommandLine
{
    /// <summary>
    /// Encodes, escapes, and optionally quotes a command-line argument that represents a file name.
    /// </summary>
    /// <param name="value">The command-line argument that represents a file name.</param>
    /// <returns>The encoded, escaped, and optionally quoted command-line argument.</returns>
    [Obsolete("Use CommandLine.EscapeArgument(CommandLine.EncodeFileName(string)) expression instead.")] // 2026
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(value))]
    public static string? EscapeFileName(string? value) => EscapeArgument(EncodeFileName(value));
}

#endif
