// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.IO.FileSystems.Kits;

/// <summary>
/// Provides a base implementation of <see cref="IFileSystem"/> interface.
/// </summary>
/// <inheritdoc/>
public abstract class FileSystemKit : VirtualFileSystemKit, IFileSystem
{
}
