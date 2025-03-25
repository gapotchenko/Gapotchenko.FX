// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Data.Archives.Zip.Tests;

partial class ZipArchiveTests
{
    protected virtual IDataArchiveFormat<IZipArchive, ZipArchiveOptions>? Format => null;

    #region Mount

    [TestMethod]
    public void Zip_Format_Mount_ClosesValidStream()
    {
        var format = Format;
        if (format == null)
            return;

        var stream = new StreamDiagnostics(Assets.OpenStream("Empty.zip"));

        format.Mount(stream, leaveOpen: true).Dispose();
        Assert.IsFalse(stream.IsClosed);
        stream.Position = 0;

        format.Mount(stream).Dispose();
        Assert.IsTrue(stream.IsClosed);
    }

    [TestMethod]
    public void Zip_Format_Mount_ClosesInvalidStream()
    {
        var format = Format;
        if (format == null)
            return;

        var stream = new StreamDiagnostics(Assets.OpenStream("Invalid/ZeroLength.zip"));

        Assert.ThrowsException<InvalidDataException>(() => format.Mount(stream, leaveOpen: true));
        Assert.IsFalse(stream.IsClosed);
        stream.Position = 0;

        Assert.ThrowsException<InvalidDataException>(() => format.Mount(stream));
        Assert.IsTrue(stream.IsClosed);
    }

    [TestMethod]
    public void Zip_Format_Mount_Empty()
    {
        var format = Format;
        if (format == null)
            return;

        using var stream = Assets.OpenStream("Empty.zip");
        Assert.IsTrue(format.IsMountable(stream));
        Assert.AreEqual(0, stream.Position);

        format.Mount(stream).Dispose();
        // TODO: restore the mount position after the unmount.
        //Assert.AreEqual(0, stream.Position);
    }

    [TestMethod]
    public void Zip_Format_Mount_Invalid_ZeroLength()
    {
        var format = Format;
        if (format == null)
            return;

        using var stream = Assets.OpenStream("Invalid/ZeroLength.zip");

        Assert.IsFalse(format.IsMountable(stream));
        Assert.ThrowsException<InvalidDataException>(() => format.Mount(stream));
    }

    [TestMethod]
    public void Zip_Format_Mount_Invalid_Noise()
    {
        var format = Format;
        if (format == null)
            return;

        using var stream = Assets.OpenStream("Invalid/Noise.zip");

        Assert.IsFalse(format.IsMountable(stream));
        Assert.ThrowsException<InvalidDataException>(() => format.Mount(stream));
    }

    #endregion

    #region Create

    [TestMethod]
    public void Zip_Format_Create_ClosesStream()
    {
        var format = Format;
        if (format == null)
            return;

        using var stream = new StreamDiagnostics(new MemoryStream());

        format.Create(stream, leaveOpen: true).Dispose();
        Assert.IsFalse(stream.IsClosed);
        stream.SetLength(0);

        format.Create(stream).Dispose();
        Assert.IsTrue(stream.IsClosed);
    }

    [TestMethod]
    public void Zip_Format_Create_Empty()
    {
        var format = Format;
        if (format == null)
            return;

        using var stream = new MemoryStream();

        format.Create(stream, leaveOpen: true).Dispose();
        Assert.AreNotEqual(0, stream.Length);
        Assert.AreEqual(stream.Length, stream.Position);
    }

    [TestMethod]
    public void Zip_Format_Create_Overwrite()
    {
        var format = Format;
        if (format == null)
            return;

        using var stream = Assets.OpenStream("Invalid/Noise.zip", true);
        long originalLength = stream.Length;

        format.Create(stream, leaveOpen: true).Dispose();
        Assert.AreNotEqual(0, stream.Length);
        Assert.AreEqual(stream.Length, stream.Position);
        Assert.IsTrue(stream.Length < originalLength);
    }

    #endregion
}
