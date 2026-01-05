using Gapotchenko.FX.Data.Integrity.Checksum.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Data.Integrity.Checksum.Tests;

[TestClass]
public class Crc8Tests
{
    static void Check19(IChecksumAlgorithm<byte> algorithm, byte check)
    {
        Assert.AreEqual(8, algorithm.ChecksumSize);
        ChecksumTestBench.Check(algorithm, ChecksumTestBench.TV19, check);
    }

    [TestMethod]
    public void Crc8_Standard_Check() => Check19(Crc8.Standard, 0xf4);

    [TestMethod]
    public void Crc8_Standard_Hash()
    {
        var algorithm = Crc8.Standard;
        var ha = algorithm.CreateHashAlgorithm();
        var hash = ha.ComputeHash(ChecksumTestBench.TV19);
        Assert.HasCount(1, hash);
        Assert.AreEqual(0xf4, hash[0]);
    }

    [TestMethod]
    public void Crc8_Tech3250_Check() => Check19(Crc8.Attested.Tech3250, 0x97);

    [TestMethod]
    public void Crc8_SaeJ1850_Check() => Check19(Crc8.Attested.SaeJ1850, 0x4b);

    [TestMethod]
    public void Crc8_OpenSafety_Check() => Check19(Crc8.Attested.OpenSafety, 0x3e);

    [TestMethod]
    public void Crc8_Nrsc5_Check() => Check19(Crc8.Attested.Nrsc5, 0xf7);

    [TestMethod]
    public void Crc8_MifareMad_Check() => Check19(Crc8.Attested.MifareMad, 0x99);

    [TestMethod]
    public void Crc8_Maxim_Check() => Check19(Crc8.Attested.Maxim, 0xa1);

    [TestMethod]
    public void Crc8_ICode_Check() => Check19(Crc8.Attested.ICode, 0x7e);

    [TestMethod]
    public void Crc8_Hitag_Check() => Check19(Crc8.Attested.Hitag, 0xb4);

    [TestMethod]
    public void Crc8_Darc_Check() => Check19(Crc8.Attested.Darc, 0x15);

    [TestMethod]
    public void Crc8_Bluetooth_Check() => Check19(Crc8.Attested.Bluetooth, 0x26);

    [TestMethod]
    public void Crc8_Autosar_Check() => Check19(Crc8.Attested.Autosar, 0xdf);

    [TestMethod]
    public void Crc8_Custom_Cdma2000_Check() =>
        Check19(
            new CustomCrc8(0x9b, 0xff, false, false, 0x00),
            0xda);
}
