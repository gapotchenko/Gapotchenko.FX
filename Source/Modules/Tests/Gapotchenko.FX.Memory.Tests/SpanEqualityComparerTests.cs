using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CA1825 // Avoid zero-length array allocations
#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0301 // Simplify collection initialization

namespace Gapotchenko.FX.Memory.Tests;

[TestClass]
public class SpanEqualityComparerTests
{
    [TestMethod]
    public void SpanEqualityComparer_NullSpansAreEqual()
    {
        Assert.IsTrue(SpanEqualityComparer.Equals((ReadOnlySpan<byte>)null, null));
        Assert.IsTrue(SpanEqualityComparer.Equals((Span<byte>)null, null));
    }

    [TestMethod]
    public void SpanEqualityComparer_NullAndEmptySpansAreNotEqual()
    {
        Assert.IsFalse(SpanEqualityComparer.Equals((ReadOnlySpan<byte>)null, Array.Empty<byte>()));
        Assert.IsFalse(SpanEqualityComparer.Equals(Array.Empty<byte>(), (ReadOnlySpan<byte>)null));

        Assert.IsFalse(SpanEqualityComparer.Equals((Span<byte>)null, Array.Empty<byte>()));
        Assert.IsFalse(SpanEqualityComparer.Equals(Array.Empty<byte>().AsSpan(), null));
    }

    [TestMethod]
    public void SpanEqualityComparer_EmptySpansAreEqual()
    {
        var empty = Array.Empty<byte>();

        Assert.IsTrue(SpanEqualityComparer.Equals(new ReadOnlySpan<byte>(empty), empty));
        Assert.IsTrue(SpanEqualityComparer.Equals(new ReadOnlySpan<byte>(new byte[0]), new byte[0]));

        Assert.IsTrue(SpanEqualityComparer.Equals(empty.AsSpan(), empty));
        Assert.IsTrue(SpanEqualityComparer.Equals((new byte[0]).AsSpan(), new byte[0]));
    }

    [TestMethod]
    public void SpanEqualityComparer_HashCodeIsDifferentBetweenNullAndEmptySpans()
    {
        Assert.AreNotEqual(
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<DateTime>)Array.Empty<DateTime>()),
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<DateTime>)null));

        Assert.AreNotEqual(
            SpanEqualityComparer.GetHashCode((Span<DateTime>)Array.Empty<DateTime>()),
            SpanEqualityComparer.GetHashCode((Span<DateTime>)null));

        // Specialized:

        Assert.AreNotEqual(
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<byte>)Array.Empty<byte>()),
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<byte>)null));

        Assert.AreNotEqual(
            SpanEqualityComparer.GetHashCode((Span<byte>)Array.Empty<byte>()),
            SpanEqualityComparer.GetHashCode((Span<byte>)null));
    }

    [TestMethod]
    public void SpanEqualityComparer_SameSpansAreEqual_Byte()
    {
        var x = new byte[] { 1, 2, 3 };
        var y = new byte[] { 1, 2, 3 };

        Assert.IsTrue(SpanEqualityComparer.Equals(new ReadOnlySpan<byte>(x), y));
        Assert.AreEqual(
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<byte>)x),
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<byte>)y));

        Assert.IsTrue(SpanEqualityComparer.Equals(new Span<byte>(x), y));
        Assert.AreEqual(
            SpanEqualityComparer.GetHashCode((Span<byte>)x),
            SpanEqualityComparer.GetHashCode((Span<byte>)y));
    }

    [TestMethod]
    public void SpanEqualityComparer_SameSpansAreEqual_Int32()
    {
        var x = new int[] { 1, 2, 3 };
        var y = new int[] { 1, 2, 3 };

        Assert.IsTrue(SpanEqualityComparer.Equals(new ReadOnlySpan<int>(x), y));
        Assert.AreEqual(
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<int>)x),
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<int>)y));

        Assert.IsTrue(SpanEqualityComparer.Equals(new Span<int>(x), y));
        Assert.AreEqual(
            SpanEqualityComparer.GetHashCode((Span<int>)x),
            SpanEqualityComparer.GetHashCode((Span<int>)y));
    }

    [TestMethod]
    public void SpanEqualityComparer_DifferentSpansAreNotEqual_Byte()
    {
        var x = new byte[] { 1, 2, 3 };
        var y = new byte[] { 3, 4, 5 };

        Assert.IsFalse(SpanEqualityComparer.Equals(new ReadOnlySpan<byte>(x), y));
        Assert.AreNotEqual(
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<byte>)x),
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<byte>)y));

        Assert.IsFalse(SpanEqualityComparer.Equals(new Span<byte>(x), y));
        Assert.AreNotEqual(
            SpanEqualityComparer.GetHashCode((Span<byte>)x),
            SpanEqualityComparer.GetHashCode((Span<byte>)y));
    }

    [TestMethod]
    public void SpanEqualityComparer_DifferentSpansAreNotEqual_Int32()
    {
        var x = new int[] { 1, 2, 3 };
        var y = new int[] { 3, 4, 5 };

        Assert.IsFalse(SpanEqualityComparer.Equals(new ReadOnlySpan<int>(x), y));
        Assert.AreNotEqual(
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<int>)x),
            SpanEqualityComparer.GetHashCode((ReadOnlySpan<int>)y));

        Assert.IsFalse(SpanEqualityComparer.Equals(new Span<int>(x), y));
        Assert.AreNotEqual(
            SpanEqualityComparer.GetHashCode((Span<int>)x),
            SpanEqualityComparer.GetHashCode((Span<int>)y));
    }
}
