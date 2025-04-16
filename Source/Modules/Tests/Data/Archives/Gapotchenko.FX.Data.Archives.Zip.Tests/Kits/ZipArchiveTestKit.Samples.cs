// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests.Kits;

partial class ZipArchiveTestKit
{
    [TestMethod]
    public void Zip_Samples_DotnetDev_1_1_4()
    {
        using var archive = Mount(Assets.OpenStream("Samples/DotnetDev.1.1.4.zip"));

        Assert.IsTrue(
            archive.DirectoryExists(archive.JoinPaths("host", "fxr")),
            "host/fxr directory not found.");
    }
}
