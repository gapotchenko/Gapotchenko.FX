// Gapotchenko.FX
//
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
    [DataRow("DotnetDev.1.1.4.zip")]
    [DataRow("DotnetSdk.2.0.0.zip")]
    public void Zip_Samples_DotnetSdk(string fileName)
    {
        using var archive = Mount(Assets.OpenStream($"Samples/{fileName}", true));

        string hostFxrPath = archive.JoinPaths("host", "fxr");

        Assert.IsTrue(
            archive.DirectoryExists(hostFxrPath),
            $"'{hostFxrPath}' directory not found.");

        Assert.AreNotEqual(
            DateTime.MinValue,
            archive.GetLastWriteTime(hostFxrPath),
            $"The last write time of '{hostFxrPath}' directory should not correspond to a non-existing entry.");

        archive.SetLastWriteTime(hostFxrPath, VfsTestContentKit.SpecialUtcTime1);
        Assert.AreEqual(VfsTestContentKit.SpecialUtcTime1, archive.GetLastWriteTime(hostFxrPath));

        Assert.ThrowsExactly<IOException>(() => archive.DeleteDirectory(hostFxrPath));
        archive.DeleteDirectory(hostFxrPath, true);
        Assert.IsFalse(
            archive.DirectoryExists(hostFxrPath),
            $"'{hostFxrPath}' directory was not deleted.");
    }
}
