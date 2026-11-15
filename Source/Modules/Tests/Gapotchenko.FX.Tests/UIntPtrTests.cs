// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Tests;

[TestClass]
public sealed class UIntPtrTests
{
    [TestMethod]
    public void UIntPtr_MinValue()
    {
        Assert.AreEqual(UIntPtr.Zero, UIntPtr.MinValue);
    }

    [TestMethod]
    public void UIntPtr_MaxValue()
    {
        nuint expected = UIntPtr.Size == 4 ? new UIntPtr(uint.MaxValue) : new UIntPtr(ulong.MaxValue);
        Assert.AreEqual<nuint>(expected, UIntPtr.MaxValue);
    }
}
