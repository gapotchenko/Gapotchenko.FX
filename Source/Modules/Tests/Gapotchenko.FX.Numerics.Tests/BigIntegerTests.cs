// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Globalization;
using System.Numerics;

namespace Gapotchenko.FX.Numerics.Tests;

[TestClass]
public sealed class BigIntegerTests
{
    [TestMethod]
    [DataRow("33022", new byte[] { 0xfe, 0x80, 0x00 }, new byte[] { 0x00, 0x80, 0xfe }, new byte[] { 0xfe, 0x80 }, new byte[] { 0x80, 0xfe })]
    public void BigInteger_ToByteArray_TV(
        string s,
        byte[] signedLE,
        byte[] signedBE,
        byte[] unsignedLE,
        byte[] unsignedBE)
    {
        var value = BigInteger.Parse(s, NumberFormatInfo.InvariantInfo);
        CollectionAssert.AreEqual(signedLE, value.ToByteArray(isUnsigned: false, isBigEndian: false));
        CollectionAssert.AreEqual(signedBE, value.ToByteArray(isUnsigned: false, isBigEndian: true));
        CollectionAssert.AreEqual(unsignedLE, value.ToByteArray(isUnsigned: true, isBigEndian: false));
        CollectionAssert.AreEqual(unsignedBE, value.ToByteArray(isUnsigned: true, isBigEndian: true));
    }
}
