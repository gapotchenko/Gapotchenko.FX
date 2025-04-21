// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Tests.Kits;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests.Kits;

partial class ZipArchiveTestKit
{
    [TestMethod]
    public void Zip_Samples_DotnetDev_1_1_4()
    {
        using var archive = Mount(Assets.OpenStream("Samples/DotnetDev.1.1.4.zip", true));

        string hostFxrPath = archive.JoinPaths("host", "fxr");

        Assert.IsTrue(
            archive.DirectoryExists(hostFxrPath),
            $"'{hostFxrPath}' directory not found.");

        Assert.AreNotEqual(
            DateTime.MinValue,
            archive.GetLastWriteTime(hostFxrPath),
            $"The last write time of '{hostFxrPath}' directory should not correspond to a non-existing entry.");

        archive.SetLastWriteTime(hostFxrPath, VfsTestContentsKit.SpecialUtcTime1);
        Assert.AreEqual(VfsTestContentsKit.SpecialUtcTime1, archive.GetLastWriteTime(hostFxrPath));

        Assert.ThrowsException<IOException>(() => archive.DeleteDirectory(hostFxrPath));
        archive.DeleteDirectory(hostFxrPath, true);
        Assert.IsFalse(
            archive.DirectoryExists(hostFxrPath),
            $"'{hostFxrPath}' directory was not deleted.");
    }
}
