// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Numerics.Tests;

partial class BitArithmeticTests
{
    #region Int32

    [TestMethod]
    [DataRow(0, 0, 4)]
    [DataRow(4, 1, 4)]
    [DataRow(4, 3, 4)]
    [DataRow(4, 4, 4)]
    [DataRow(8, 5, 4)]
    [DataRow(0, -1, 4)]
    [DataRow(0, -3, 4)]
    [DataRow(-4, -4, 4)]
    [DataRow(-4, -5, 4)]
    [DataRow(int.MinValue, int.MinValue, 1)]
    [DataRow(int.MaxValue, int.MaxValue, 1)]
    public void AlignUp_Int32(int expected, int value, int alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(3)]
    [DataRow(6)]
    public void AlignUp_Int32_InvalidAlignment(int alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignUp(0, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0u, 0u, 4u)]
    [DataRow(4u, 1u, 4u)]
    [DataRow(4u, 3u, 4u)]
    [DataRow(4u, 4u, 4u)]
    [DataRow(8u, 5u, 4u)]
    [DataRow(0x80000000u, 0x7fffffffu, 0x80000000u)]
    [DataRow(uint.MaxValue, uint.MaxValue, 1u)]
    public void AlignUp_UInt32(uint expected, uint value, uint alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(0u)]
    [DataRow(3u)]
    [DataRow(6u)]
    [DataRow(uint.MaxValue)]
    public void AlignUp_UInt32_InvalidAlignment(uint alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignUp(0u, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0, 0, 4)]
    [DataRow(4, 1, 4)]
    [DataRow(4, 4, 4)]
    [DataRow(0, -1, 4)]
    [DataRow(-4, -5, 4)]
    [DataRow(int.MaxValue, int.MaxValue, 1)]
    public void TryAlignUp_Int32(int expected, int value, int alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.TryAlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(int.MaxValue, 2)]
    [DataRow(int.MaxValue - 1, 4)]
    [DataRow(0x40000001, 0x40000000)]
    public void TryAlignUp_Int32_Overflow(int value, int alignment)
    {
        Assert.IsNull(BitArithmetic.TryAlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(3)]
    [DataRow(6)]
    public void TryAlignUp_Int32_InvalidAlignment(int alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.TryAlignUp(0, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0, 0, 4)]
    [DataRow(4, 1, 4)]
    [DataRow(4, 4, 4)]
    [DataRow(0, -1, 4)]
    [DataRow(-4, -5, 4)]
    [DataRow(int.MaxValue, int.MaxValue, 1)]
    public void AlignUpChecked_Int32(int expected, int value, int alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignUpChecked(value, alignment));
    }

    [TestMethod]
    [DataRow(int.MaxValue, 2)]
    [DataRow(int.MaxValue - 1, 4)]
    [DataRow(0x40000001, 0x40000000)]
    public void AlignUpChecked_Int32_Overflow(int value, int alignment)
    {
        Assert.ThrowsExactly<OverflowException>(() => BitArithmetic.AlignUpChecked(value, alignment));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(3)]
    [DataRow(6)]
    public void AlignUpChecked_Int32_InvalidAlignment(int alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignUpChecked(0, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0, 0, 4)]
    [DataRow(0, 1, 4)]
    [DataRow(0, 3, 4)]
    [DataRow(4, 4, 4)]
    [DataRow(4, 5, 4)]
    [DataRow(-4, -1, 4)]
    [DataRow(-4, -3, 4)]
    [DataRow(-4, -4, 4)]
    [DataRow(-8, -5, 4)]
    [DataRow(int.MinValue, int.MinValue, 1)]
    [DataRow(int.MaxValue, int.MaxValue, 1)]
    public void AlignDown_Int32(int expected, int value, int alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignDown(value, alignment));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(3)]
    [DataRow(6)]
    public void AlignDown_Int32_InvalidAlignment(int alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignDown(0, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(true, 0, 4)]
    [DataRow(false, 1, 4)]
    [DataRow(false, 3, 4)]
    [DataRow(true, 4, 4)]
    [DataRow(false, 5, 4)]
    [DataRow(false, -1, 4)]
    [DataRow(false, -3, 4)]
    [DataRow(true, -4, 4)]
    [DataRow(false, -5, 4)]
    [DataRow(true, int.MinValue, 1)]
    [DataRow(true, int.MaxValue, 1)]
    public void IsAligned_Int32(bool expected, int value, int alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.IsAligned(value, alignment));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(3)]
    [DataRow(6)]
    public void IsAligned_Int32_InvalidAlignment(int alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.IsAligned(0, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0, 0, 4)]
    [DataRow(3, 1, 4)]
    [DataRow(1, 3, 4)]
    [DataRow(0, 4, 4)]
    [DataRow(1, -1, 4)]
    [DataRow(3, -3, 4)]
    [DataRow(0, -4, 4)]
    [DataRow(1, int.MaxValue, 2)]
    [DataRow(0, int.MinValue, 4)]
    [DataRow(0x3fffffff, 1, 0x40000000)]
    [DataRow(0, int.MinValue, 1)]
    [DataRow(0, int.MaxValue, 1)]
    public void GetAlignmentPadding_Int32(int expected, int value, int alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.GetAlignmentPadding(value, alignment));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(3)]
    [DataRow(6)]
    public void GetAlignmentPadding_Int32_InvalidAlignment(int alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            BitArithmetic.GetAlignmentPadding(0, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    #endregion

    #region UInt32

    [TestMethod]
    [DataRow(0u, 0u, 4u)]
    [DataRow(4u, 1u, 4u)]
    [DataRow(4u, 4u, 4u)]
    [DataRow(0x80000000u, 0x7fffffffu, 0x80000000u)]
    [DataRow(uint.MaxValue, uint.MaxValue, 1u)]
    public void TryAlignUp_UInt32(uint expected, uint value, uint alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.TryAlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(uint.MaxValue, 2u)]
    [DataRow(uint.MaxValue - 1, 4u)]
    [DataRow(0x80000001u, 0x80000000u)]
    public void TryAlignUp_UInt32_Overflow(uint value, uint alignment)
    {
        Assert.IsNull(BitArithmetic.TryAlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(0u)]
    [DataRow(3u)]
    [DataRow(6u)]
    [DataRow(uint.MaxValue)]
    public void TryAlignUp_UInt32_InvalidAlignment(uint alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.TryAlignUp(0u, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0u, 0u, 4u)]
    [DataRow(4u, 1u, 4u)]
    [DataRow(4u, 4u, 4u)]
    [DataRow(0x80000000u, 0x7fffffffu, 0x80000000u)]
    [DataRow(uint.MaxValue, uint.MaxValue, 1u)]
    public void AlignUpChecked_UInt32(uint expected, uint value, uint alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignUpChecked(value, alignment));
    }

    [TestMethod]
    [DataRow(uint.MaxValue, 2u)]
    [DataRow(uint.MaxValue - 1, 4u)]
    [DataRow(0x80000001u, 0x80000000u)]
    public void AlignUpChecked_UInt32_Overflow(uint value, uint alignment)
    {
        Assert.ThrowsExactly<OverflowException>(() => BitArithmetic.AlignUpChecked(value, alignment));
    }

    [TestMethod]
    [DataRow(0u)]
    [DataRow(3u)]
    [DataRow(6u)]
    [DataRow(uint.MaxValue)]
    public void AlignUpChecked_UInt32_InvalidAlignment(uint alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignUpChecked(0u, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0u, 0u, 4u)]
    [DataRow(0u, 1u, 4u)]
    [DataRow(0u, 3u, 4u)]
    [DataRow(4u, 4u, 4u)]
    [DataRow(4u, 5u, 4u)]
    [DataRow(0u, 0x7fffffffu, 0x80000000u)]
    [DataRow(uint.MaxValue, uint.MaxValue, 1u)]
    public void AlignDown_UInt32(uint expected, uint value, uint alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignDown(value, alignment));
    }

    [TestMethod]
    [DataRow(0u)]
    [DataRow(3u)]
    [DataRow(6u)]
    [DataRow(uint.MaxValue)]
    public void AlignDown_UInt32_InvalidAlignment(uint alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignDown(0u, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(true, 0u, 4u)]
    [DataRow(false, 1u, 4u)]
    [DataRow(false, 3u, 4u)]
    [DataRow(true, 4u, 4u)]
    [DataRow(false, 5u, 4u)]
    [DataRow(false, 0x7fffffffu, 0x80000000u)]
    [DataRow(true, 0x80000000u, 0x80000000u)]
    [DataRow(true, uint.MaxValue, 1u)]
    public void IsAligned_UInt32(bool expected, uint value, uint alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.IsAligned(value, alignment));
    }

    [TestMethod]
    [DataRow(0u)]
    [DataRow(3u)]
    [DataRow(6u)]
    [DataRow(uint.MaxValue)]
    public void IsAligned_UInt32_InvalidAlignment(uint alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.IsAligned(0u, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0u, 0u, 4u)]
    [DataRow(3u, 1u, 4u)]
    [DataRow(1u, 3u, 4u)]
    [DataRow(0u, 4u, 4u)]
    [DataRow(1u, uint.MaxValue, 2u)]
    [DataRow(0x7fffffffu, 1u, 0x80000000u)]
    [DataRow(1u, uint.MaxValue, 0x80000000u)]
    [DataRow(0u, 0u, 1u)]
    [DataRow(0u, uint.MaxValue, 1u)]
    public void GetAlignmentPadding_UInt32(uint expected, uint value, uint alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.GetAlignmentPadding(value, alignment));
    }

    [TestMethod]
    [DataRow(0u)]
    [DataRow(3u)]
    [DataRow(6u)]
    [DataRow(uint.MaxValue)]
    public void GetAlignmentPadding_UInt32_InvalidAlignment(uint alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            BitArithmetic.GetAlignmentPadding(0u, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    #endregion

    #region Int64

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(8L, 5L, 4L)]
    [DataRow(0L, -3L, 4L)]
    [DataRow(long.MinValue, long.MaxValue, 2L)]
    [DataRow(long.MaxValue, long.MaxValue, 1L)]
    public void AlignUp_Int64(long expected, long value, long alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(0L)]
    [DataRow(-1L)]
    [DataRow(3L)]
    [DataRow(6L)]
    public void AlignUp_Int64_InvalidAlignment(long alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignUp(0L, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(8L, 5L, 4L)]
    [DataRow(0L, -3L, 4L)]
    [DataRow(long.MaxValue, long.MaxValue, 1L)]
    public void TryAlignUp_Int64(long expected, long value, long alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.TryAlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(long.MaxValue, 2L)]
    [DataRow(long.MaxValue - 1, 4L)]
    [DataRow(0x4000000000000001L, 0x4000000000000000L)]
    public void TryAlignUp_Int64_Overflow(long value, long alignment)
    {
        Assert.IsNull(BitArithmetic.TryAlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(0L)]
    [DataRow(-1L)]
    [DataRow(3L)]
    [DataRow(6L)]
    public void TryAlignUp_Int64_InvalidAlignment(long alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.TryAlignUp(0L, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(8L, 5L, 4L)]
    [DataRow(0L, -3L, 4L)]
    [DataRow(long.MaxValue, long.MaxValue, 1L)]
    public void AlignUpChecked_Int64(long expected, long value, long alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignUpChecked(value, alignment));
    }

    [TestMethod]
    [DataRow(long.MaxValue, 2L)]
    [DataRow(long.MaxValue - 1, 4L)]
    [DataRow(0x4000000000000001L, 0x4000000000000000L)]
    public void AlignUpChecked_Int64_Overflow(long value, long alignment)
    {
        Assert.ThrowsExactly<OverflowException>(() => BitArithmetic.AlignUpChecked(value, alignment));
    }

    [TestMethod]
    [DataRow(0L)]
    [DataRow(-1L)]
    [DataRow(3L)]
    [DataRow(6L)]
    public void AlignUpChecked_Int64_InvalidAlignment(long alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignUpChecked(0L, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(4L, 5L, 4L)]
    [DataRow(-4L, -3L, 4L)]
    [DataRow(long.MinValue, long.MinValue, 1L)]
    [DataRow(long.MaxValue, long.MaxValue, 1L)]
    public void AlignDown_Int64(long expected, long value, long alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignDown(value, alignment));
    }

    [TestMethod]
    [DataRow(0L)]
    [DataRow(-1L)]
    [DataRow(3L)]
    [DataRow(6L)]
    public void AlignDown_Int64_InvalidAlignment(long alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignDown(0L, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(true, 0L, 4L)]
    [DataRow(false, 3L, 4L)]
    [DataRow(true, -4L, 4L)]
    [DataRow(false, -3L, 4L)]
    [DataRow(true, long.MaxValue, 1L)]
    public void IsAligned_Int64(bool expected, long value, long alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.IsAligned(value, alignment));
    }

    [TestMethod]
    [DataRow(0L)]
    [DataRow(-1L)]
    [DataRow(3L)]
    [DataRow(6L)]
    public void IsAligned_Int64_InvalidAlignment(long alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.IsAligned(0L, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(3L, 1L, 4L)]
    [DataRow(1L, -1L, 4L)]
    [DataRow(1L, long.MaxValue, 2L)]
    [DataRow(0x3fffffffffffffffL, 1L, 0x4000000000000000L)]
    public void GetAlignmentPadding_Int64(long expected, long value, long alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.GetAlignmentPadding(value, alignment));
    }

    [TestMethod]
    [DataRow(0L)]
    [DataRow(-1L)]
    [DataRow(3L)]
    [DataRow(6L)]
    public void GetAlignmentPadding_Int64_InvalidAlignment(long alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.GetAlignmentPadding(0L, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    #endregion

    #region UInt64

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(8UL, 5UL, 4UL)]
    [DataRow(0UL, ulong.MaxValue, 2UL)]
    [DataRow(ulong.MaxValue, ulong.MaxValue, 1UL)]
    public void AlignUp_UInt64(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(0UL)]
    [DataRow(3UL)]
    [DataRow(6UL)]
    [DataRow(ulong.MaxValue)]
    public void AlignUp_UInt64_InvalidAlignment(ulong alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignUp(0UL, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(8UL, 5UL, 4UL)]
    [DataRow(ulong.MaxValue, ulong.MaxValue, 1UL)]
    public void TryAlignUp_UInt64(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.TryAlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(ulong.MaxValue, 2UL)]
    [DataRow(ulong.MaxValue - 1, 4UL)]
    [DataRow(0x8000000000000001UL, 0x8000000000000000UL)]
    public void TryAlignUp_UInt64_Overflow(ulong value, ulong alignment)
    {
        Assert.IsNull(BitArithmetic.TryAlignUp(value, alignment));
    }

    [TestMethod]
    [DataRow(0UL)]
    [DataRow(3UL)]
    [DataRow(6UL)]
    [DataRow(ulong.MaxValue)]
    public void TryAlignUp_UInt64_InvalidAlignment(ulong alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.TryAlignUp(0UL, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(8UL, 5UL, 4UL)]
    [DataRow(ulong.MaxValue, ulong.MaxValue, 1UL)]
    public void AlignUpChecked_UInt64(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignUpChecked(value, alignment));
    }

    [TestMethod]
    [DataRow(ulong.MaxValue, 2UL)]
    [DataRow(ulong.MaxValue - 1, 4UL)]
    [DataRow(0x8000000000000001UL, 0x8000000000000000UL)]
    public void AlignUpChecked_UInt64_Overflow(ulong value, ulong alignment)
    {
        Assert.ThrowsExactly<OverflowException>(() => BitArithmetic.AlignUpChecked(value, alignment));
    }

    [TestMethod]
    [DataRow(0UL)]
    [DataRow(3UL)]
    [DataRow(6UL)]
    [DataRow(ulong.MaxValue)]
    public void AlignUpChecked_UInt64_InvalidAlignment(ulong alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignUpChecked(0UL, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(4UL, 5UL, 4UL)]
    [DataRow(0UL, 0x7fffffffffffffffUL, 0x8000000000000000UL)]
    [DataRow(ulong.MaxValue, ulong.MaxValue, 1UL)]
    public void AlignDown_UInt64(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.AlignDown(value, alignment));
    }

    [TestMethod]
    [DataRow(0UL)]
    [DataRow(3UL)]
    [DataRow(6UL)]
    [DataRow(ulong.MaxValue)]
    public void AlignDown_UInt64_InvalidAlignment(ulong alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.AlignDown(0UL, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(true, 0UL, 4UL)]
    [DataRow(false, 3UL, 4UL)]
    [DataRow(true, 0x8000000000000000UL, 0x8000000000000000UL)]
    [DataRow(false, ulong.MaxValue, 0x8000000000000000UL)]
    public void IsAligned_UInt64(bool expected, ulong value, ulong alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.IsAligned(value, alignment));
    }

    [TestMethod]
    [DataRow(0UL)]
    [DataRow(3UL)]
    [DataRow(6UL)]
    [DataRow(ulong.MaxValue)]
    public void IsAligned_UInt64_InvalidAlignment(ulong alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.IsAligned(0UL, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(3UL, 1UL, 4UL)]
    [DataRow(1UL, ulong.MaxValue, 2UL)]
    [DataRow(0x7fffffffffffffffUL, 1UL, 0x8000000000000000UL)]
    public void GetAlignmentPadding_UInt64(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.GetAlignmentPadding(value, alignment));
    }

    [TestMethod]
    [DataRow(0UL)]
    [DataRow(3UL)]
    [DataRow(6UL)]
    [DataRow(ulong.MaxValue)]
    public void GetAlignmentPadding_UInt64_InvalidAlignment(ulong alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => BitArithmetic.GetAlignmentPadding(0UL, alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    #endregion

    #region IntPtr

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(8L, 5L, 4L)]
    [DataRow(0L, -3L, 4L)]
    public void AlignUp_IntPtr(long expected, long value, long alignment)
    {
        Assert.AreEqual((nint)expected, BitArithmetic.AlignUp((nint)value, (nint)alignment));
    }

    [TestMethod]
    public void AlignUp_IntPtr_Overflow()
    {
        Assert.AreEqual(nint.MinValue, BitArithmetic.AlignUp(nint.MaxValue, 2));
    }

    [TestMethod]
    [DataRow(0L)]
    [DataRow(-1L)]
    [DataRow(3L)]
    [DataRow(6L)]
    public void AlignUp_IntPtr_InvalidAlignment(long alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            BitArithmetic.AlignUp(0, (nint)alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(8L, 5L, 4L)]
    [DataRow(0L, -3L, 4L)]
    public void TryAlignUp_IntPtr(long expected, long value, long alignment)
    {
        Assert.AreEqual((nint)expected, BitArithmetic.TryAlignUp((nint)value, (nint)alignment));
    }

    [TestMethod]
    public void TryAlignUp_IntPtr_Overflow()
    {
        Assert.IsNull(BitArithmetic.TryAlignUp(nint.MaxValue, 2));
    }

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(8L, 5L, 4L)]
    [DataRow(0L, -3L, 4L)]
    public void AlignUpChecked_IntPtr(long expected, long value, long alignment)
    {
        Assert.AreEqual((nint)expected, BitArithmetic.AlignUpChecked((nint)value, (nint)alignment));
    }

    [TestMethod]
    public void AlignUpChecked_IntPtr_Overflow()
    {
        Assert.ThrowsExactly<OverflowException>(() => BitArithmetic.AlignUpChecked(nint.MaxValue, 2));
    }

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(4L, 5L, 4L)]
    [DataRow(-4L, -3L, 4L)]
    public void AlignDown_IntPtr(long expected, long value, long alignment)
    {
        Assert.AreEqual((nint)expected, BitArithmetic.AlignDown((nint)value, (nint)alignment));
    }

    [TestMethod]
    [DataRow(true, 0L, 4L)]
    [DataRow(false, 3L, 4L)]
    [DataRow(true, -4L, 4L)]
    [DataRow(false, -3L, 4L)]
    public void IsAligned_IntPtr(bool expected, long value, long alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.IsAligned((nint)value, (nint)alignment));
    }

    [TestMethod]
    [DataRow(0L, 0L, 4L)]
    [DataRow(3L, 1L, 4L)]
    [DataRow(1L, -1L, 4L)]
    public void GetAlignmentPadding_IntPtr(long expected, long value, long alignment)
    {
        Assert.AreEqual((nint)expected, BitArithmetic.GetAlignmentPadding((nint)value, (nint)alignment));
    }

    [TestMethod]
    public void GetAlignmentPadding_IntPtr_MaxValue()
    {
        Assert.AreEqual(1, BitArithmetic.GetAlignmentPadding(nint.MaxValue, 2));
    }

    #endregion

    #region UIntPtr

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(8UL, 5UL, 4UL)]
    public void AlignUp_UIntPtr(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual((nuint)expected, BitArithmetic.AlignUp((nuint)value, (nuint)alignment));
    }

    [TestMethod]
    public void AlignUp_UIntPtr_Overflow()
    {
        Assert.AreEqual((nuint)0, BitArithmetic.AlignUp(nuint.MaxValue, 2));
    }

    [TestMethod]
    [DataRow(0UL)]
    [DataRow(3UL)]
    [DataRow(6UL)]
    public void AlignUp_UIntPtr_InvalidAlignment(ulong alignment)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            BitArithmetic.AlignUp(0, (nuint)alignment));
        Assert.AreEqual("alignment", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(8UL, 5UL, 4UL)]
    public void TryAlignUp_UIntPtr(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual((nuint)expected, BitArithmetic.TryAlignUp((nuint)value, (nuint)alignment));
    }

    [TestMethod]
    public void TryAlignUp_UIntPtr_Overflow()
    {
        Assert.IsNull(BitArithmetic.TryAlignUp(nuint.MaxValue, 2));
    }

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(8UL, 5UL, 4UL)]
    public void AlignUpChecked_UIntPtr(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual((nuint)expected, BitArithmetic.AlignUpChecked((nuint)value, (nuint)alignment));
    }

    [TestMethod]
    public void AlignUpChecked_UIntPtr_Overflow()
    {
        Assert.ThrowsExactly<OverflowException>(() =>
            BitArithmetic.AlignUpChecked(nuint.MaxValue, 2));
    }

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(4UL, 5UL, 4UL)]
    public void AlignDown_UIntPtr(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual((nuint)expected, BitArithmetic.AlignDown((nuint)value, (nuint)alignment));
    }

    [TestMethod]
    [DataRow(true, 0UL, 4UL)]
    [DataRow(false, 3UL, 4UL)]
    public void IsAligned_UIntPtr(bool expected, ulong value, ulong alignment)
    {
        Assert.AreEqual(expected, BitArithmetic.IsAligned((nuint)value, (nuint)alignment));
    }

    [TestMethod]
    [DataRow(0UL, 0UL, 4UL)]
    [DataRow(3UL, 1UL, 4UL)]
    public void GetAlignmentPadding_UIntPtr(ulong expected, ulong value, ulong alignment)
    {
        Assert.AreEqual((nuint)expected, BitArithmetic.GetAlignmentPadding((nuint)value, (nuint)alignment));
    }

    [TestMethod]
    public void GetAlignmentPadding_UIntPtr_MaxValue()
    {
        Assert.AreEqual((nuint)1, BitArithmetic.GetAlignmentPadding(nuint.MaxValue, 2));
    }

    #endregion
}
