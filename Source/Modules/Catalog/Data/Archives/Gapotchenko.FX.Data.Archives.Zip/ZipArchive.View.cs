// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Data.Archives.Zip;

partial class ZipArchive
{
    /// <inheritdoc cref="CreateView(System.IO.Compression.ZipArchive, bool)"/>
    public static IZipArchiveView<System.IO.Compression.ZipArchive> CreateView(System.IO.Compression.ZipArchive archive) =>
        CreateView(archive, false);

    /// <summary>
    /// Creates a virtual file-system view for the specified <see cref="System.IO.Compression.ZipArchive"/> instance.
    /// </summary>
    /// <param name="archive">The <see cref="System.IO.Compression.ZipArchive"/> instance to create the view of.</param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the underlying <paramref name="archive"/> open
    /// after the created <see cref="IZipArchiveView{T}"/> view object is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>The instance of a virtual file-system view for the <paramref name="archive"/>.</returns>
    public static IZipArchiveView<System.IO.Compression.ZipArchive> CreateView(System.IO.Compression.ZipArchive archive, bool leaveOpen) =>
        new ZipArchiveViewOnBcl(archive, leaveOpen);
}
