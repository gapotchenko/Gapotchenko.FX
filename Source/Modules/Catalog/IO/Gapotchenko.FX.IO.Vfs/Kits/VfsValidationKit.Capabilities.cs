// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.IO.Vfs.Kits;

partial class VfsValidationKit
{
    /// <summary>
    /// Provides capability validation facilities.
    /// </summary>
    [StackTraceHidden]
    public static class Capabilities
    {
        /// <summary>
        /// Ensures that the file system can open a file with the specified mode and access.
        /// </summary>
        /// <param name="mode">The file mode.</param>
        /// <param name="access">The file access.</param>
        /// <param name="canRead">The value indicating whether the file system supports reading.</param>
        /// <param name="canWrite">The value indicating whether the file system supports writing.</param>
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
}
