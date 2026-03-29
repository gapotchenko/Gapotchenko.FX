// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © Open MCDF Project
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Tests;

[TestClass]
public sealed class MSCfbOpenMcdfTests
{
    #region Stream

    [TestMethod]
    [DataRow("Open MCDF/TestStream_v3_0.cfb", 0)]
    [DataRow("Open MCDF/TestStream_v3_63.cfb", 63)]
    [DataRow("Open MCDF/TestStream_v3_64.cfb", 64)]
    [DataRow("Open MCDF/TestStream_v3_65.cfb", 65)]
    [DataRow("Open MCDF/TestStream_v3_511.cfb", 511)]
    [DataRow("Open MCDF/TestStream_v3_512.cfb", 512)]
    [DataRow("Open MCDF/TestStream_v3_513.cfb", 513)]
    [DataRow("Open MCDF/TestStream_v3_4095.cfb", 4095)]
    [DataRow("Open MCDF/TestStream_v3_4096.cfb", 4096)]
    [DataRow("Open MCDF/TestStream_v3_4097.cfb", 4097)]
    public void MSCfb_OpenMcdf_Stream_Read(string fileName, int expectedLength)
    {
        using var assetStream = Assets.OpenStream(fileName);
        using var vfs = new MSCfbFileSystem(assetStream);

        Assert.AreEqual(expectedLength, vfs.GetFileSize("/TestStream"));

        using var stream = vfs.OpenFile("/TestStream", FileMode.Open, FileAccess.Read, FileShare.Read);
        Assert.AreEqual(expectedLength, stream.Length);

        // Test files are filled with bytes equal to their position modulo 256.
        for (int i = 0; i < expectedLength; i++)
        {
            int value = stream.ReadByte();
            Assert.AreNotEqual(-1, value, $"Unexpected end of stream at position {i}.");
            Assert.AreEqual((byte)i, (byte)value, $"Unexpected byte value at position {i}.");
        }

        Assert.AreEqual(-1, stream.ReadByte(), "Expected end of stream.");
    }

    #endregion

    #region Storage

    [TestMethod]
    [DataRow("Open MCDF/MultipleStorage.cfb", 1)]
    [DataRow("Open MCDF/MultipleStorage2.cfb", 1)]
    [DataRow("Open MCDF/MultipleStorage3.cfb", 1)]
    [DataRow("Open MCDF/MultipleStorage4.cfb", 1)]
    public void MSCfb_OpenMcdf_Storage_Enumerate(string fileName, int expectedCount)
    {
        using var assetStream = Assets.OpenStream(fileName);
        using var vfs = new MSCfbFileSystem(assetStream);

        int actualCount = vfs.EnumerateDirectories("/").Count();
        Assert.AreEqual(expectedCount, actualCount);
    }

    [TestMethod]
    public void MSCfb_OpenMcdf_Storage_OpenNamed()
    {
        using var assetStream = Assets.OpenStream("Open MCDF/MultipleStorage.cfb");
        using var vfs = new MSCfbFileSystem(assetStream);

        Assert.IsTrue(vfs.DirectoryExists("/MyStorage"));
        Assert.IsFalse(vfs.DirectoryExists("/NonExistentStorage"));
    }

    #endregion

    #region Samples

    [TestMethod]
    [DataRow("Open MCDF/Samples/LibreOfficeBlankSample_v25.8.doc")]
    [DataRow("Open MCDF/Samples/LibreOfficeBlankSample_v25.8.xls")]
    [DataRow("Open MCDF/Samples/LibreOfficeBlankSample_v25.8.ppt")]
    [DataRow("Open MCDF/Samples/Office365BlankSample_v2507.doc")]
    [DataRow("Open MCDF/Samples/Office365BlankSample_v2507.xls")]
    [DataRow("Open MCDF/Samples/Office365BlankSample_v2507.ppt")]
    public void MSCfb_OpenMcdf_Sample_Open(string fileName)
    {
        using var assetStream = Assets.OpenStream(fileName);
        using var vfs = new MSCfbFileSystem(assetStream);

        var entries = vfs.EnumerateEntries("/");
        Assert.IsTrue(entries.Any(), "Expected at least one entry in the root storage.");
    }

    #endregion
}
