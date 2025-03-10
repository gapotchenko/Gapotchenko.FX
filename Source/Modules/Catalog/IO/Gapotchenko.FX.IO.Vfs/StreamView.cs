// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides static methods for working with virtual views on a <see cref="Stream"/>.
/// </summary>
public static class StreamView
{
    /// <summary>
    /// Gets a view on the specified stream with capabilities enforced according to the specified values.
    /// </summary>
    /// <param name="stream">The base to enforce the capabilities for.</param>
    /// <param name="canRead">Indicates whether the stream should support reading.</param>
    /// <param name="canWrite">Indicates whether the stream should support writing.</param>
    /// <param name="canSeek">Indicates whether the stream should support seeking.</param>
    /// <returns>
    /// The view on the <paramref name="stream"/> that enforces the specified capabilities,
    /// or the <paramref name="stream"/> itself if it already matches them or is <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(stream))]
    public static Stream? WithCapabilities(Stream? stream, bool canRead, bool canWrite, bool canSeek)
    {
        if (stream is null)
        {
            return null;
        }
        else if (!canRead && stream.CanRead ||
            !canWrite && stream.CanWrite ||
            !canSeek && stream.CanSeek)
        {
            return new StreamViewWithCapabilities(stream, canRead, canWrite, canSeek);
        }
        else
        {
            return stream;
        }
    }
}
