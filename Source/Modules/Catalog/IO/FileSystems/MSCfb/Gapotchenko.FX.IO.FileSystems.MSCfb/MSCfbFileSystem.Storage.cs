// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb;

partial class MSCfbFileSystem
{
    /// <summary>
    /// Gets the object for MS-CFB storage manipulation.
    /// </summary>
    public static IFileSystemStorage<IMSCfbFileSystem, MSCfbFileSystemOptions> Storage => MSCfbFileSystemStorage.Instance;

#if TFF_STATIC_INTERFACE
    static IVfsStorage<IMSCfbFileSystem, MSCfbFileSystemOptions> IStorageMountableVfs<IMSCfbFileSystem, MSCfbFileSystemOptions>.Storage => Storage;
#endif
}
