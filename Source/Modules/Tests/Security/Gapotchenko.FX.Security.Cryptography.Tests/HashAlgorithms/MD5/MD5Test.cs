// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography.Tests.HashAlgorithms.MD5;

using MD5 = System.Security.Cryptography.MD5;

public abstract class MD5Test : HashAlgorithmTest
{
    /// <remarks>
    /// Test cases are defined in RFC 1321, section A.5.
    /// </remarks>
    [TestMethod]
    [DataRow("", "d41d8cd98f00b204e9800998ecf8427e")]
    [DataRow("a", "0cc175b9c0f1b6a831c399e269772661")]
    [DataRow("abc", "900150983cd24fb0d6963f7d28e17f72")]
    [DataRow("message digest", "f96b697d7cb7938d525a2f31aaf161d0")]
    [DataRow("abcdefghijklmnopqrstuvwxyz", "c3fcd3d76192e4007dfb496cca67e13b")]
    [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "d174ab98d277d9f5a5611c2c9f419d9f")]
    [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890", "57edf4a22be3c955ac49da2e2107b67a")]
    public void MD5_TV_RFC1321(string inputText, string expectedHex) => Verify(inputText, expectedHex);

    // ------------------------------------------------------------------------

    protected sealed override HashAlgorithm CreateHashAlgorithm() => CreateAlgorithm();

    protected abstract MD5 CreateAlgorithm();
}
