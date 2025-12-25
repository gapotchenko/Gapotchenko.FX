// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides enumerated values to use to set file-system search expression options.
/// </summary>
[Flags]
public enum VfsSearchExpressionOptions
{
    /// <summary>
    /// Specifies that no options are set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Specifies case-insensitive matching.
    /// </summary>
    IgnoreCase = 1 << 0
}
