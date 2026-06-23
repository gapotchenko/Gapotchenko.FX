// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
// Portions © The Mono Project
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Data.Encoding;
using System.Security.Cryptography;
using System.Text;

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms.DES;

using DES = System.Security.Cryptography.DES;

public abstract class DESTestsBase : SymmetricAlgorithmTestsBase
{
    #region Keys

    const string KnownWeakKeyHex = "e0e0e0e0f1f1f1f1";
    protected static readonly byte[] KnownWeakKey = Base16.GetBytes(KnownWeakKeyHex);

    [TestMethod]
    [DataRow(KnownWeakKeyHex)]
    [DataRow("0101010101010101")]
    [DataRow("fefefefefefefefe")]
    [DataRow("1f1f1f1f0e0e0e0e")]
    public void DES_Key_Weak(string s)
    {
        byte[] key = Base16.GetBytes(s);
        Assert.IsTrue(IsWeakKey(key));
        Assert.IsFalse(IsSemiWeakKey(key));

        Assert.ThrowsExactly<CryptographicException>(() => IsWeakKey(null!));
        Assert.ThrowsExactly<CryptographicException>(() => IsWeakKey(KnownShortKey));
    }

    const string KnownSemiWeakKeyHex = "1f011f010e010e01";
    protected static readonly byte[] KnownSemiWeakKey = Base16.GetBytes(KnownSemiWeakKeyHex);

    [TestMethod]
    [DataRow(KnownSemiWeakKeyHex)]
    [DataRow("01fe01fe01fe01fe")]
    [DataRow("fe01fe01fe01fe01")]
    [DataRow("1fe01fe00ef10ef1")]
    [DataRow("e01fe01ff10ef10e")]
    [DataRow("01e001e001f101f1")]
    [DataRow("e001e001f101f101")]
    [DataRow("1ffe1ffe0efe0efe")]
    [DataRow("fe1ffe1ffe0efe0e")]
    [DataRow("011f011f010e010e")]
    [DataRow("e0fee0fef1fef1fe")]
    [DataRow("fee0fee0fef1fef1")]
    public void DES_Key_SemiWeak(string s)
    {
        byte[] key = Base16.GetBytes(s);
        Assert.IsTrue(IsSemiWeakKey(key));
        Assert.IsFalse(IsWeakKey(key));

        Assert.ThrowsExactly<CryptographicException>(() => IsSemiWeakKey(null!));
        Assert.ThrowsExactly<CryptographicException>(() => IsSemiWeakKey(KnownShortKey));
    }

    const string KnownGoodKeyHex = "87ff0737f868378f";
    protected static readonly byte[] KnownGoodKey = Base16.GetBytes(KnownGoodKeyHex);

    [TestMethod]
    [DataRow(KnownGoodKeyHex)]
    public void DES_Key_Good(string s)
    {
        byte[] key = Base16.GetBytes(s);
        Assert.IsFalse(IsWeakKey(key));
        Assert.IsFalse(IsSemiWeakKey(key));
    }

    protected static readonly byte[] KnownShortKey = Base16.GetBytes("00");

    [TestMethod]
    public void DES_Key_Size()
    {
        using var des = CreateAlgorithm();
        Assert.AreEqual(64, des.KeySize);

        Assert.ThrowsExactly<CryptographicException>(() => des.KeySize = 64 - 8);
        Assert.ThrowsExactly<CryptographicException>(() => des.KeySize = 64 + 8);
    }

    #endregion

    #region Blocks

    [TestMethod]
    public void DES_Block_Size()
    {
        using var des = CreateAlgorithm();
        Assert.AreEqual(64, des.KeySize);

        des.BlockSize = 64;
        Assert.AreEqual(64, des.BlockSize);

        Assert.ThrowsExactly<CryptographicException>(() => des.BlockSize = 63);
        Assert.ThrowsExactly<CryptographicException>(() => des.BlockSize = 65);
    }

    #endregion

    #region Cipher

    [TestMethod]
    public void DES_Cipher_Key()
    {
        using var des = CreateAlgorithm();

        if (ThrowsCryptographicExceptionOnInvalidCipherArguments)
        {
            Assert.ThrowsExactly<CryptographicException>(() => des.CreateDecryptor(KnownShortKey, des.IV));
            Assert.ThrowsExactly<CryptographicException>(() => des.CreateEncryptor(KnownShortKey, des.IV));
        }
        else
        {
            Assert.ThrowsExactly<ArgumentException>(() => des.CreateDecryptor(KnownShortKey, des.IV));
            Assert.ThrowsExactly<ArgumentException>(() => des.CreateEncryptor(KnownShortKey, des.IV));
        }

        Assert.ThrowsExactly<CryptographicException>(() => des.CreateDecryptor(KnownWeakKey, des.IV));
        Assert.ThrowsExactly<CryptographicException>(() => des.CreateDecryptor(KnownSemiWeakKey, des.IV));

        Assert.ThrowsExactly<CryptographicException>(() => des.CreateEncryptor(KnownWeakKey, des.IV));
        Assert.ThrowsExactly<CryptographicException>(() => des.CreateEncryptor(KnownSemiWeakKey, des.IV));
    }

    static readonly byte[] m_MultiBlockString = Encoding.ASCII.GetBytes("This is a sentence that is longer than a block, it ensures that multi-block functions work.");

    static readonly byte[] m_MultiBlockStringPaddedZeros = Base16.GetBytes("""
        5468697320697320612073656E74656E63652074686174206973206C6F6E6765
        72207468616E206120626C6F636B2C20697420656E7375726573207468617420
        6D756C74692D626C6F636B2066756E6374696F6E7320776F726B2E0000000000
        """);

    static readonly byte[] m_MultiBlockString_8 = Encoding.ASCII.GetBytes("This is a sentence that is longer than a block,but exactly an even block multiplier of 8");

    static readonly byte[] m_RandomKey = Base16.GetBytes("87FF0737F868378F");
    static readonly byte[] m_RandomIV = Base16.GetBytes("E531E789E3E1BB6F");

    static IEnumerable<(CipherMode, PaddingMode, byte[] Key, byte[]? IV, byte[] Plain, byte[]? ExpectedDecrypted, byte[] ExpectedEncrypted)>
        DES_Cipher_RoundTrip_Data
    {
        get
        {
            // FIPS81 ECB with plaintext "Now is the time for all "
            yield return
            (
                CipherMode.ECB,
                PaddingMode.None,
                Base16.GetBytes("0123456789abcdef"),
                null,
                Base16.GetBytes("4e6f77206973207468652074696d6520666f7220616c6c20"),
                null,
                Base16.GetBytes("3fa40e8a984d48156a271787ab8883f9893d51ec4b563b53")
            );

            yield return
            (
                CipherMode.ECB,
                PaddingMode.None,
                m_RandomKey,
                null,
                m_MultiBlockString_8,
                null,
                Base16.GetBytes("""
                    4E42A439ED50C7998CD626B8BE1ECC0A82B985EA772030E87C96BFAE1B97A766
                    6505B8AE96745DE2921F6868897C20F2BC8C7B284FD1E9A0A2E49DDAB7A39782
                    33423377C88177CB2D92475EE4DC1FF9E6DFA135DE648E1B
                    """)
            );

            yield return
            (
                CipherMode.ECB,
                PaddingMode.PKCS7,
                m_RandomKey,
                null,
                m_MultiBlockString,
                null,
                Base16.GetBytes("""
                    4E42A439ED50C7998CD626B8BE1ECC0A82B985EA772030E87C96BFAE1B97A766
                    6505B8AE96745DE249C1EC3338BBAD4193A9B792205F345E22D45A9A996F21CE
                    24697E5A45F600E8C6E71FC7114A3E96EC4EACC9F652DEBC679D22DE7141F67F
                    """)
            );

            yield return
            (
                CipherMode.ECB,
                PaddingMode.Zeros,
                m_RandomKey,
                null,
                m_MultiBlockString,
                m_MultiBlockStringPaddedZeros,
                Base16.GetBytes("""
                    4E42A439ED50C7998CD626B8BE1ECC0A82B985EA772030E87C96BFAE1B97A766
                    6505B8AE96745DE249C1EC3338BBAD4193A9B792205F345E22D45A9A996F21CE
                    24697E5A45F600E8C6E71FC7114A3E96EC4EACC9F652DEBC471DF9564F29C738
                    """)
            );

            // FIPS81 CBC with plaintext "Now is the time for all "
            yield return
            (
                CipherMode.CBC,
                PaddingMode.None,
                Base16.GetBytes("0123456789abcdef"),
                Base16.GetBytes("1234567890abcdef"),
                Base16.GetBytes("4e6f77206973207468652074696d6520666f7220616c6c20"),
                null,
                Base16.GetBytes("e5c7cdde872bf27c43e934008c389c0f683788499a7c05f6")
            );

            yield return
            (
                CipherMode.CBC,
                PaddingMode.None,
                m_RandomKey,
                m_RandomIV,
                m_MultiBlockString_8,
                null,
                Base16.GetBytes("""
                    7264319AE3C504148CD4A19B4FDC7D2ACCCB0A08D60CBE2B885DCB2C1A86ED9C
                    A51006E33859B03EEB61CF5219D769C1ABF1A1FDE0EF87D3B3C4D567D9C8960D
                    DA55DBE13341928FEF38B938E1F62FAD1D05E355E440E012
                    """)
            );

            yield return
            (
                CipherMode.CBC,
                PaddingMode.Zeros,
                m_RandomKey,
                m_RandomIV,
                m_MultiBlockString,
                m_MultiBlockStringPaddedZeros,
                Base16.GetBytes("""
                    7264319AE3C504148CD4A19B4FDC7D2ACCCB0A08D60CBE2B885DCB2C1A86ED9C
                    A51006E33859B03E00F5B57801EFF745F7A577842461CF39AC143505EC326233
                    E66343A46FEADE9E8456D8AC6A84A1C32E6792857F062400740CB21A333D334D
                    """)
            );

            yield return
            (
                CipherMode.CBC,
                PaddingMode.PKCS7,
                m_RandomKey,
                m_RandomIV,
                m_MultiBlockString,
                null,
                Base16.GetBytes("""
                    7264319AE3C504148CD4A19B4FDC7D2ACCCB0A08D60CBE2B885DCB2C1A86ED9C
                    A51006E33859B03E00F5B57801EFF745F7A577842461CF39AC143505EC326233
                    E66343A46FEADE9E8456D8AC6A84A1C32E6792857F062400EA9053D17AD3C35D
                    """)
            );
        }
    }

    [TestMethod]
    [DynamicData(nameof(DES_Cipher_RoundTrip_Data))]
    public void DES_Cipher_RoundTrip(CipherMode cipherMode, PaddingMode paddingMode, byte[] key, byte[]? iv, byte[] plain, byte[]? expectedDecrypted, byte[] expectedEncrypted)
    {
        expectedDecrypted ??= plain;

        using var algorithm = CreateAlgorithm();
        algorithm.Key = key;
        algorithm.Padding = paddingMode;
        algorithm.Mode = cipherMode;
        if (iv != null)
            algorithm.IV = iv;

        byte[] cipher = algorithm.Encrypt(plain);
        CollectionAssert.AreEqual(expectedEncrypted, cipher);

        byte[] decrypted = algorithm.Decrypt(cipher);
        CollectionAssert.AreEqual(expectedDecrypted, decrypted);
    }

    [TestMethod]
    public void DES_Cipher_EncryptExplicitWithIV()
    {
        using var algorithm = CreateAlgorithm();
        algorithm.Padding = PaddingMode.PKCS7;
        algorithm.Mode = CipherMode.CBC;

        using var encryptor = algorithm.CreateEncryptor(m_RandomKey, m_RandomIV);
        byte[] plainText = m_MultiBlockString;
        byte[] actualCipher = encryptor.Transform(plainText);
        byte[] expectedCipher = Base16.GetBytes("""            
            7264319AE3C504148CD4A19B4FDC7D2ACCCB0A08D60CBE2B885DCB2C1A86ED9C
            A51006E33859B03E00F5B57801EFF745F7A577842461CF39AC143505EC326233
            E66343A46FEADE9E8456D8AC6A84A1C32E6792857F062400EA9053D17AD3C35D            
            """);
        CollectionAssert.AreEqual(expectedCipher, actualCipher);
    }

    [TestMethod]
    public void DES_Cipher_EncryptExplicitWithoutIV()
    {
        using var algorithm = CreateAlgorithm();
        algorithm.Padding = PaddingMode.PKCS7;
        algorithm.Mode = CipherMode.ECB;

        using var encryptor = algorithm.CreateEncryptor(m_RandomKey, null);
        byte[] plainText = m_MultiBlockString;
        byte[] actualCipher = encryptor.Transform(plainText);
        byte[] expectedCipher = Base16.GetBytes("""
            4E42A439ED50C7998CD626B8BE1ECC0A82B985EA772030E87C96BFAE1B97A766
            6505B8AE96745DE249C1EC3338BBAD4193A9B792205F345E22D45A9A996F21CE
            24697E5A45F600E8C6E71FC7114A3E96EC4EACC9F652DEBC679D22DE7141F67F
            """);
        CollectionAssert.AreEqual(expectedCipher, actualCipher);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DES_Cipher_EncryptLargeOutputBuffer(bool blockAlignedOutput)
    {
        using var algorithm = CreateAlgorithm();
        using var encryptor = algorithm.CreateEncryptor();

        // 8 blocks, plus maybe three bytes
        int outputPadding = blockAlignedOutput ? 0 : 3;
        byte[] output = new byte[algorithm.BlockSize + outputPadding];
        // 2 blocks of 0x00
        byte[] input = new byte[algorithm.BlockSize / 4];
        int outputOffset = 0;

        outputOffset += encryptor.TransformBlock(input, 0, input.Length, output, outputOffset);
        byte[] overflow = encryptor.TransformFinalBlock([], 0, 0);
        Buffer.BlockCopy(overflow, 0, output, outputOffset, overflow.Length);
        outputOffset += overflow.Length;

        Assert.AreEqual(3 * (algorithm.BlockSize / 8), outputOffset);

        string outputAsHex = Base16.GetString(output);
        Assert.AreNotEqual(
            new string('0', outputOffset * 2),
            outputAsHex[..(outputOffset * 2)]);
        Assert.AreEqual(
            new string('0', (output.Length - outputOffset) * 2),
            outputAsHex[(outputOffset * 2)..]);
    }

    [TestMethod]
    [DataRow(true, true)]
    [DataRow(true, false)]
    [DataRow(false, true)]
    [DataRow(false, false)]
    public void DES_Cipher_TooShortOutputBuffer(bool encrypt, bool blockAlignedOutput)
    {
        using var algorithm = CreateAlgorithm();
        using var transform = encrypt ? algorithm.CreateEncryptor() : algorithm.CreateDecryptor();

        // 1 block, plus maybe three bytes
        int outputPadding = blockAlignedOutput ? 0 : 3;
        byte[] output = new byte[algorithm.BlockSize / 8 + outputPadding];
        // 3 blocks of 0x00
        byte[] input = new byte[3 * (algorithm.BlockSize / 8)];

        void DoTransform()
        {
            transform.TransformBlock(input, 0, input.Length, output, 0);
        }

        if (ThrowsCryptographicExceptionOnInvalidCipherArguments)
            Assert.ThrowsExactly<CryptographicException>(DoTransform);
        else
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(DoTransform);

        CollectionAssert.AreEqual(new byte[output.Length], output);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DES_Cipher_DecryptBlock(bool blockAlignedOutput)
    {
        const string expectedOutput = "This is a test";

        int outputPadding = blockAlignedOutput ? 0 : 3;
        byte[] key = Base16.GetBytes("87FF0737F868378F");
        byte[] iv = Base16.GetBytes("0123456789ABCDEF");
        byte[] outputBytes = new byte[iv.Length * 2 + outputPadding];
        byte[] input = Base16.GetBytes("CB67F70BA8B50EED2C0691298988865F");
        int outputOffset = 0;

        using var algorithm = CreateAlgorithm();

        using (var decryptor = algorithm.CreateDecryptor(key, iv))
        {
            Assert.AreEqual(2 * algorithm.BlockSize, (outputBytes.Length - outputPadding) * 8);
            outputOffset += decryptor.TransformBlock(input, 0, input.Length, outputBytes, outputOffset);
            byte[] overflow = decryptor.TransformFinalBlock([], 0, 0);
            Buffer.BlockCopy(overflow, 0, outputBytes, outputOffset, overflow.Length);
            outputOffset += overflow.Length;
        }

        string decrypted = Encoding.ASCII.GetString(outputBytes, 0, outputOffset);
        Assert.AreEqual(expectedOutput, decrypted);
    }

    #endregion

    // ------------------------------------------------------------------------

    protected override SymmetricAlgorithm CreateSymmetricAlgorithm() => CreateAlgorithm();

    protected abstract DES CreateAlgorithm();

    protected abstract bool IsWeakKey(byte[] key);

    protected abstract bool IsSemiWeakKey(byte[] key);
}
