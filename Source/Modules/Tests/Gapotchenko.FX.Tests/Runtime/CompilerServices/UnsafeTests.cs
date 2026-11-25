// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Tests.Runtime.CompilerServices;

[TestClass]
public sealed class UnsafeTests
{
    [TestMethod]
    public void Unsafe_BitCast()
    {
        const int bits = 0x3f800000;

        float value = Unsafe.BitCast<int, float>(bits);

        Assert.AreEqual(1.0f, value);
        Assert.AreEqual(bits, Unsafe.BitCast<float, int>(value));
    }

    [TestMethod]
    public void Unsafe_BitCast_DifferentSizes()
    {
        Assert.ThrowsExactly<NotSupportedException>(() => Unsafe.BitCast<int, long>(0));
    }
}
