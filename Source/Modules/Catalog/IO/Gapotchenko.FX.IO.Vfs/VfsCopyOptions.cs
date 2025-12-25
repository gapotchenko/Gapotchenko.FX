// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Defines the options for file and directory copy operations provided by a virtual file system.
/// </summary>
[Flags]
public enum VfsCopyOptions
{
    /// <summary>
    /// No options.
    /// </summary>
    None = 0,

    /// <summary>
    /// Preserve as much as possible of the structure and attributes of the original files in the copy.
    /// </summary>
    Archive = 1 << 0
}
