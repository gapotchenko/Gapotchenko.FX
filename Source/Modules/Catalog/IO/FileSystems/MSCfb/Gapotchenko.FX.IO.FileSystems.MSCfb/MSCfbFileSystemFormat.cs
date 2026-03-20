// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;
using Gapotchenko.FX.IO.FileSystems.Kits;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb;

sealed class MSCfbFileSystemFormat : FileSystemFormatKit<IMSCfbFileSystem, MSCfbFileSystemOptions>
{
    public static MSCfbFileSystemFormat Instance { get; } = new();

    protected override IReadOnlyList<string> GetFileExtensionsCore() => [];

    protected override IMSCfbFileSystem CreateCore(Stream stream, bool leaveOpen, MSCfbFileSystemOptions? options, VfsStorageContext? context) =>
        new MSCfbFileSystem(stream, true, leaveOpen, options, context, create: true);

    protected override IMSCfbFileSystem MountCore(Stream stream, bool writable, bool leaveOpen, MSCfbFileSystemOptions? options, VfsStorageContext? context) =>
        new MSCfbFileSystem(stream, writable, leaveOpen, options, context);

    protected override bool IsMountableCore(Stream stream, MSCfbFileSystemOptions? options, VfsStorageContext? context)
    {
        Span<byte> buffer = stackalloc byte[CfbConstants.Signature.Length];
        return
            stream.ReadAtLeast(buffer, buffer.Length, throwOnEndOfStream: false) == buffer.Length &&
            buffer.SequenceEqual(CfbConstants.Signature);
    }

    protected override async Task<bool> IsMountableCoreAsync(Stream stream, MSCfbFileSystemOptions? options, VfsStorageContext? context, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[CfbConstants.Signature.Length];
        return
            await stream.ReadAtLeastAsync(buffer, buffer.Length, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(false) == buffer.Length &&
            buffer.SequenceEqual(CfbConstants.Signature);
    }
}
