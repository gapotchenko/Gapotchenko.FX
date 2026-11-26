// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Buffers.Binary;
using System.Buffers.Binary;

namespace Gapotchenko.FX.Memory.Tests.Buffers.Binary;

[TestClass]
public sealed class BinaryPrimitivesTests
{
    [TestMethod]
    public void BinaryPrimitives_SingleLittleEndian()
    {
        byte[] buffer = new byte[sizeof(float)];

        BinaryPrimitives.WriteSingleLittleEndian(buffer, 1.0f);

        Assert.AreSequenceEqual<byte>([0x00, 0x00, 0x80, 0x3f], buffer);
        Assert.AreEqual(1.0f, BinaryPrimitives.ReadSingleLittleEndian(buffer));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BinaryPrimitives.ReadSingleLittleEndian([]));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BinaryPrimitives.WriteSingleLittleEndian([], 0));
    }

    [TestMethod]
    public void BinaryPrimitives_SingleBigEndian()
    {
        byte[] buffer = new byte[sizeof(float)];

        BinaryPrimitives.WriteSingleBigEndian(buffer, 1.0f);

        Assert.AreSequenceEqual<byte>([0x3f, 0x80, 0x00, 0x00], buffer);
        Assert.AreEqual(1.0f, BinaryPrimitives.ReadSingleBigEndian(buffer));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BinaryPrimitives.ReadSingleBigEndian([]));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BinaryPrimitives.WriteSingleBigEndian([], 0));
    }

    [TestMethod]
    public void BinaryPrimitives_DoubleLittleEndian()
    {
        byte[] buffer = new byte[sizeof(double)];

        BinaryPrimitives.WriteDoubleLittleEndian(buffer, 1.0);

        Assert.AreSequenceEqual<byte>([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0x3f], buffer);
        Assert.AreEqual(1.0, BinaryPrimitives.ReadDoubleLittleEndian(buffer));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BinaryPrimitives.ReadDoubleLittleEndian([]));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BinaryPrimitives.WriteDoubleLittleEndian([], 0));
    }

    [TestMethod]
    public void BinaryPrimitives_DoubleBigEndian()
    {
        byte[] buffer = new byte[sizeof(double)];

        BinaryPrimitives.WriteDoubleBigEndian(buffer, 1.0);

        Assert.AreSequenceEqual<byte>([0x3f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00], buffer);
        Assert.AreEqual(1.0, BinaryPrimitives.ReadDoubleBigEndian(buffer));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BinaryPrimitives.ReadDoubleBigEndian([]));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BinaryPrimitives.WriteDoubleBigEndian([], 0));
    }
}
