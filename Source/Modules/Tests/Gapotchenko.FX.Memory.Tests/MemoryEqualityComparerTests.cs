using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Memory.Tests;

[TestClass]
public class MemoryEqualityComparerTests
{
    [TestMethod]
    public void MemoryEqualityComparer_1()
    {
        byte[] bytes = [1, 2, 3, 4, 5, 6];
        var memory = bytes.AsMemory();

        var map = new Dictionary<ReadOnlyMemory<byte>, string>(MemoryEqualityComparer<byte>.Default)
        {
            [memory.Slice(0, 3)] = "A",
            [memory.Slice(3, 3)] = "B"
        };

        Assert.AreEqual("A", map[new byte[] { 1, 2, 3 }]);
        Assert.AreEqual("B", map[new byte[] { 4, 5, 6 }]);

        Assert.IsFalse(map.ContainsKey(new byte[] { 1, 2 }));
        Assert.IsFalse(map.ContainsKey(new byte[] { 3, 4 }));
    }
}
