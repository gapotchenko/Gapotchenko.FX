namespace Gapotchenko.FX.Memory.Tests;

[TestClass]
public class MemoryEqualityComparerTests
{
    [TestMethod]
    public void MemoryEqualityComparer_Dictionary_ValueType()
    {
        byte[] bytes = [1, 2, 3, 4, 5, 6];
        var memory = bytes.AsMemory();

        var map = new Dictionary<ReadOnlyMemory<byte>, string>(MemoryEqualityComparer<byte>.Default)
        {
            [memory[0..3]] = "A",
            [memory[3..6]] = "B"
        };

        Assert.AreEqual("A", map[new byte[] { 1, 2, 3 }]);
        Assert.AreEqual("B", map[new byte[] { 4, 5, 6 }]);

        Assert.IsFalse(map.ContainsKey(new byte[] { 1, 2 }));
        Assert.IsFalse(map.ContainsKey(new byte[] { 3, 4 }));
    }

    [TestMethod]
    public void MemoryEqualityComparer_Dictionary_Reference()
    {
        string?[] strings = ["1", null, "3", "4", null, "6"];
        var memory = strings.AsMemory();

        var map = new Dictionary<ReadOnlyMemory<string?>, string>(MemoryEqualityComparer<string?>.Default)
        {
            [memory[0..3]] = "A",
            [memory[3..6]] = "B"
        };

        Assert.AreEqual("A", map[new[] { "1", null, "3" }]);
        Assert.AreEqual("B", map[new[] { "4", null, "6" }]);

        Assert.IsFalse(map.ContainsKey(new[] { "1", "2" }));
        Assert.IsFalse(map.ContainsKey(new[] { "3", "4" }));
    }

    [TestMethod]
    public void MemoryEqualityComparer_NullAndEmptyRegionsAreNotEqual()
    {
        ReadOnlyMemory<byte> m1 = null;
        ReadOnlyMemory<byte> m2 = Array.Empty<byte>();

        Assert.IsFalse(MemoryEqualityComparer.Equals(m1, m2));
        Assert.IsFalse(MemoryEqualityComparer.Equals(m2, m1));
    }
}
