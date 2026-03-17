// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO;
using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;
using Gapotchenko.FX.IO.FileSystems.Kits;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb;

sealed class MSCfbFileSystemFormat : FileSystemFormatKit<IMSCfbFileSystem, MSCfbFileSystemOptions>
{
    public static MSCfbFileSystemFormat Instance { get; } = new();

    protected override IReadOnlyList<string> GetFileExtensionsCore() => [];

    protected override IMSCfbFileSystem CreateCore(Stream stream, bool leaveOpen, MSCfbFileSystemOptions? options, VfsStorageContext? context) =>
        new MSCfbFileSystem(stream, leaveOpen, options, context, create: true);

    protected override IMSCfbFileSystem MountCore(Stream stream, bool writable, bool leaveOpen, MSCfbFileSystemOptions? options, VfsStorageContext? context) =>
        new MSCfbFileSystem(stream, leaveOpen, options, context, writable);

    protected override bool IsMountableCore(Stream stream, MSCfbFileSystemOptions? options, VfsStorageContext? context)
    {
        byte[] buffer = new byte[CfbConstants.Signature.Length];
        return
            stream.ReadAtLeast(buffer, buffer.Length, throwOnEndOfStream: false) == buffer.Length &&
            buffer.AsSpan().SequenceEqual(CfbConstants.Signature);
    }
}
