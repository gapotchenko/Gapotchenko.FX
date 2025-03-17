// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Utils;

namespace Gapotchenko.FX.IO.Vfs;

static class FileSystemViewCapabilities
{
    /// <summary>
    /// Ensures that the file system can open a file with the specified mode and access.
    /// </summary>
    /// <exception cref="NotSupportedException">File system does not support reading.</exception>
    /// <exception cref="NotSupportedException">File system does not support writing.</exception>
    public static void EnsureCanOpenFile(
        FileMode mode,
        FileAccess access,
        bool canRead,
        bool canWrite)
    {
        if (!canRead &&
            ((access & FileAccess.Read) != 0 ||
            mode is FileMode.Open or FileMode.OpenOrCreate))
        {
            ThrowHelper.CannotReadFS();
        }

        if (!canWrite &&
            ((access & FileAccess.Write) != 0 ||
            mode is FileMode.Create or FileMode.CreateNew or FileMode.OpenOrCreate or FileMode.Truncate or FileMode.Append))
        {
            ThrowHelper.CannotWriteFS();
        }
    }
}
