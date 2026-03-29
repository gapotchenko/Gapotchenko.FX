// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.FileSystems.MSCfb;

/// <summary>
/// Defines the options of an MS-CFB file system.
/// </summary>
[ImmutableObject(true)]
public sealed record MSCfbFileSystemOptions : FileSystemOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MSCfbFileSystemOptions"/> record.
    /// </summary>
    public MSCfbFileSystemOptions()
    {
    }
}
