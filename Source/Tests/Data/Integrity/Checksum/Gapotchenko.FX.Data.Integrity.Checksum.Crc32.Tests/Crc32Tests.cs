using Gapotchenko.FX.Data.Integrity.Checksum.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Data.Integrity.Checksum.Tests;

[TestClass]
public class Crc32Tests
{
    static void Check19(IChecksumAlgorithm<uint> algorithm, uint check)
    {
        Assert.AreEqual(32, algorithm.ChecksumSize);
        ChecksumTestBench.Check(algorithm, ChecksumTestBench.TV19, check);
    }

    [TestMethod]
    public void Crc32_Standard_Check() => Check19(Crc32.Standard, 0xcbf43926);

    [TestMethod]
    public void Crc32_Standard_Hash()
    {
        var algorithm = Crc32.Standard;
        var ha = algorithm.CreateHashAlgorithm();
        var hash = ha.ComputeHash(ChecksumTestBench.TV19);
        Assert.AreEqual(4, hash.Length);
        Assert.AreEqual(0xcbf43926, LittleEndianBitConverter.ToUInt32(hash));
    }

    void Crc32_Standard_Hash(IBitConverter bitConverter)
    {
        var algorithm = Crc32.Standard;
        var ha = algorithm.CreateHashAlgorithm(bitConverter);
        var hash = ha.ComputeHash(ChecksumTestBench.TV19);
        Assert.AreEqual(4, hash.Length);
        Assert.AreEqual(0xcbf43926, bitConverter.ToUInt32(hash));
    }

    [TestMethod]
    public void Crc32_Standard_Hash_LE() => Crc32_Standard_Hash(LittleEndianBitConverter.Instance);

    [TestMethod]
    public void Crc32_Standard_Hash_BE() => Crc32_Standard_Hash(BigEndianBitConverter.Instance);

    [TestMethod]
    public void Crc32_C_Check() => Check19(Crc32.Attested.C, 0xe3069283);

    [TestMethod]
    public void Crc32_Autosar_Check() => Check19(Crc32.Attested.Autosar, 0x1697d06a);

    [TestMethod]
    public void Crc32_Posix_Check() => Check19(Crc32.Attested.Posix, 0x765e7680);

    [TestMethod]
    public void Crc32_DectB_Check() => Check19(Crc32.Attested.BZip2, 0xfc891918);

    [TestMethod]
    public void Crc32_Mef_Check() => Check19(Crc32.Attested.Mef, 0xd2c22f51);

    [TestMethod]
    public void Crc32_Mpeg2_Check() => Check19(Crc32.Attested.Mpeg2, 0x0376e6e7);

    [TestMethod]
    public void Crc32_Custom_JamCrc_Check() =>
        Check19(
            new CustomCrc32(0x04c11db7, 0xffffffff, true, true, 0),
            0x340bc6d9);

    [TestMethod]
    public void Crc32_Custom_JamCrc_0_Check() =>
        Check19(
            new CustomCrc32(0x04c11db7, 0, true, true, 0),
            0x2dfd2d88);
}
