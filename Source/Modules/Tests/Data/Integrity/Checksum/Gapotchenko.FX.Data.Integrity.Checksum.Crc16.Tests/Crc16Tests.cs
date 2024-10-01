using Gapotchenko.FX.Data.Integrity.Checksum.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Data.Integrity.Checksum.Tests;

[TestClass]
public class Crc16Tests
{
    static void Check19(IChecksumAlgorithm<ushort> algorithm, ushort check)
    {
        Assert.AreEqual(16, algorithm.ChecksumSize);
        ChecksumTestBench.Check(algorithm, ChecksumTestBench.TV19, check);
    }

    [TestMethod]
    public void Crc16_Standard_Check() => Check19(Crc16.Standard, 0xbb3d);

    [TestMethod]
    public void Crc16_Standard_Hash()
    {
        var algorithm = Crc16.Standard;
        var ha = algorithm.CreateHashAlgorithm();
        var hash = ha.ComputeHash(ChecksumTestBench.TV19);
        Assert.AreEqual(2, hash.Length);
        Assert.AreEqual(0xbb3d, LittleEndianBitConverter.ToUInt16(hash));
    }

    void Crc16_Standard_Hash(IBitConverter bitConverter)
    {
        var algorithm = Crc16.Standard;
        var ha = algorithm.CreateHashAlgorithm(bitConverter);
        var hash = ha.ComputeHash(ChecksumTestBench.TV19);
        Assert.AreEqual(2, hash.Length);
        Assert.AreEqual(0xbb3d, bitConverter.ToUInt16(hash));
    }

    [TestMethod]
    public void Crc16_Standard_Hash_LE() => Crc16_Standard_Hash(LittleEndianBitConverter.Instance);

    [TestMethod]
    public void Crc16_Standard_Hash_BE() => Crc16_Standard_Hash(BigEndianBitConverter.Instance);

    [TestMethod]
    public void Crc16_Ccitt_Check() => Check19(Crc16.Attested.Ccitt, 0x2189);

    [TestMethod]
    public void Crc16_IsoIec14443_3_A_Check() => Check19(Crc16.Attested.IsoIec14443_3_A, 0xbf05);

    [TestMethod]
    public void Crc16_IsoIec14443_3_B_Check() => Check19(Crc16.Attested.IsoIec14443_3_B, 0x906e);

    [TestMethod]
    public void Crc16_Maxim_Check() => Check19(Crc16.Attested.Maxim, 0x44c2);

    [TestMethod]
    public void Crc16_Nrsc5_Check() => Check19(Crc16.Attested.Nrsc5, 0xa066);

    [TestMethod]
    public void Crc16_SpiFujitsu_Check() => Check19(Crc16.Attested.SpiFujitsu, 0xe5cc);

    [TestMethod]
    public void Crc16_Umts_Check() => Check19(Crc16.Attested.Umts, 0xfee8);

    [TestMethod]
    public void Crc16_Usb_Check() => Check19(Crc16.Attested.Usb, 0xb4c8);

    [TestMethod]
    public void Crc16_XModem_Check() => Check19(Crc16.Attested.XModem, 0x31c3);

    [TestMethod]
    public void Crc16_Profibus_Check() => Check19(Crc16.Attested.Profibus, 0xa819);

    [TestMethod]
    public void Crc16_Modbus_Check() => Check19(Crc16.Attested.Modbus, 0x4b37);

    [TestMethod]
    public void Crc16_Genibus_Check() => Check19(Crc16.Attested.Genibus, 0xd64e);

    [TestMethod]
    public void Crc16_Gsm_Check() => Check19(Crc16.Attested.Gsm, 0xce3c);

    [TestMethod]
    public void Crc16_OpenSafetyA_Check() => Check19(Crc16.Attested.OpenSafetyA, 0x5d38);

    [TestMethod]
    public void Crc16_OpenSafetyB_Check() => Check19(Crc16.Attested.OpenSafetyB, 0x20fe);

    [TestMethod]
    public void Crc16_TMS37157_Check() => Check19(Crc16.Attested.TMS37157, 0x26b1);

    [TestMethod]
    public void Crc16_MCRF4XX_Check() => Check19(Crc16.Attested.MCRF4xx, 0x6f91);

    [TestMethod]
    public void Crc16_DectR_Check() => Check19(Crc16.Attested.DectR, 0x007e);

    [TestMethod]
    public void Crc16_DectX_Check() => Check19(Crc16.Attested.DectX, 0x007f);

    [TestMethod]
    public void Crc16_Dds110_Check() => Check19(Crc16.Attested.Dds110, 0x9ecf);

    [TestMethod]
    public void Crc16_CcittFalse_Check() => Check19(Crc16.Attested.CcittFalse, 0x29b1);
}
