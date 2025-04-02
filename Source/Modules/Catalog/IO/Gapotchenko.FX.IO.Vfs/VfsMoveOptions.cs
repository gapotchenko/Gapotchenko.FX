// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Defines the options for file and directory move operations provided by a virtual file system.
/// </summary>
public enum VfsMoveOptions
{
    /// <summary>
    /// No options.
    /// </summary>
    None = 0,

#if TODO
    // Research the behavior of 'mv' command from GNU coreutils 9.5+.

    /// <summary>
    /// Exchange source and destination instead of moving source to destination.
    /// </summary>
    /// <remarks>
    /// This option can be used to replace one file or directory with another.
    /// Exchanges are atomic if the source and destination are both in a single file system that supports atomic exchange.
    /// </remarks>
    Exchange = 1 << 0
#endif
}
