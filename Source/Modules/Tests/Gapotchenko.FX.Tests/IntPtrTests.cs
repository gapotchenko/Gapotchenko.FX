// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Tests;

[TestClass]
public sealed class IntPtrTests
{
    [TestMethod]
    public void IntPtr_MinValue()
    {
        nint expected = IntPtr.Size == 4 ? new IntPtr(int.MinValue) : new IntPtr(long.MinValue);
        Assert.AreEqual<nint>(expected, IntPtr.MinValue);
    }

    [TestMethod]
    public void IntPtr_MaxValue()
    {
        nint expected = IntPtr.Size == 4 ? new IntPtr(int.MaxValue) : new IntPtr(long.MaxValue);
        Assert.AreEqual<nint>(expected, IntPtr.MaxValue);
    }
}
