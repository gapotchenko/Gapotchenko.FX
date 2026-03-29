// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © The Apache Software Foundation
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Tests;

[TestClass]
public sealed class MSCfbApachePoiTests
{
    [TestMethod]
    [DataRow("Apache POI/document/Word6.doc")]
    [DataRow("Apache POI/document/hyperlink.doc")]
    [DataRow("Apache POI/document/table-merges.doc")]
    public void MSCfb_ApachePOI_Document_Open(string fileName)
    {
        using var assetStream = Assets.OpenStream(fileName);
        using var vfs = new MSCfbFileSystem(assetStream);

        Assert.IsTrue(vfs.FileExists("/WordDocument"));
    }

    [TestMethod]
    [DataRow("Apache POI/spreadsheet/1900DateWindowing.xls")]
    [DataRow("Apache POI/spreadsheet/SimpleWithColours.xls")]
    public void MSCfb_ApachePOI_Spreadsheet_Open(string fileName)
    {
        using var assetStream = Assets.OpenStream(fileName);
        using var vfs = new MSCfbFileSystem(assetStream);

        Assert.IsTrue(vfs.FileExists("/Workbook"));
    }

    [TestMethod]
    public void MSCfb_ApachePOI_Spreadsheet_BookStream()
    {
        // This file uses a non-standard stream name "BOOK" (all caps) instead of "Workbook".
        using var assetStream = Assets.OpenStream("Apache POI/spreadsheet/BOOK_in_capitals.xls");
        using var vfs = new MSCfbFileSystem(assetStream);

        Assert.IsTrue(vfs.FileExists("/BOOK"));
        Assert.IsFalse(vfs.FileExists("/Workbook"));
    }

    [TestMethod]
    [DataRow("Apache POI/slideshow/Single_Coloured_Page.ppt")]
    [DataRow("Apache POI/slideshow/empty.ppt")]
    [DataRow("Apache POI/slideshow/incorrect_slide_order.ppt")]
    public void MSCfb_ApachePOI_Presentation_Open(string fileName)
    {
        using var assetStream = Assets.OpenStream(fileName);
        using var vfs = new MSCfbFileSystem(assetStream);

        Assert.IsTrue(vfs.FileExists("/PowerPoint Document"));
    }

    [TestMethod]
    [DataRow("Apache POI/hsmf/blank.msg")]
    [DataRow("Apache POI/hsmf/ASCII_CP1251_LCID1049.msg")]
    [DataRow("Apache POI/hsmf/ASCII_UTF-8_CP1252_LCID1031.msg")]
    [DataRow("Apache POI/hsmf/simple_test_msg.msg")]
    public void MSCfb_ApachePOI_Message_Open(string fileName)
    {
        using var assetStream = Assets.OpenStream(fileName);
        using var vfs = new MSCfbFileSystem(assetStream);

        Assert.IsTrue(vfs.FileExists("/__properties_version1.0"));
        Assert.IsTrue(vfs.DirectoryExists("/__nameid_version1.0"));
    }

    [TestMethod]
    public void MSCfb_ApachePOI_ZeroByteStreams()
    {
        using var assetStream = Assets.OpenStream("Apache POI/poifs/only-zero-byte-streams.ole2");
        using var vfs = new MSCfbFileSystem(assetStream);

        Assert.AreEqual(3, vfs.EnumerateFiles("/").Count());

        Assert.AreEqual(0L, vfs.GetFileSize("/test-zero-1"));
        Assert.AreEqual(0L, vfs.GetFileSize("/test-zero-2"));
        Assert.AreEqual(0L, vfs.GetFileSize("/test-zero-3"));
    }

    [TestMethod]
    public void MSCfb_ApachePOI_Notes_Open()
    {
        using var assetStream = Assets.OpenStream("Apache POI/poifs/Notes.ole2");
        using var vfs = new MSCfbFileSystem(assetStream);

        // Notes.ole2 contains a single unnamed storage at the root that embeds an OLE 1.0 native object.
        Assert.AreEqual(1, vfs.EnumerateDirectories("/").Count());
    }
}
