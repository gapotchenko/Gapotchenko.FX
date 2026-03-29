// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb;

sealed class MSCfbFileSystemStorage : IFileSystemStorage<IMSCfbFileSystem, MSCfbFileSystemOptions>
{
    public static MSCfbFileSystemStorage Instance { get; } = new();

    public IFileSystemFormat<IMSCfbFileSystem, MSCfbFileSystemOptions> Format => MSCfbFileSystemFormat.Instance;

    IVfsFileStorageFormat<IMSCfbFileSystem, MSCfbFileSystemOptions> IVfsStorage<IMSCfbFileSystem, MSCfbFileSystemOptions>.Format => Format;
}
