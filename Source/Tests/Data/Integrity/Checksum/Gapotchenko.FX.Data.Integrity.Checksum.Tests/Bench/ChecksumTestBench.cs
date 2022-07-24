using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Gapotchenko.FX.Data.Integrity.Checksum.Tests.Bench;

public static class ChecksumTestBench
{
    /// <summary>
    /// Gets the test vector corresponding to the bytes of ASCII string "123456789".
    /// </summary>
    public static byte[] TV19 { get; } = Encoding.ASCII.GetBytes("123456789");

    public static void Check<T>(IChecksumAlgorithm<T> algorithm, byte[] data, T check)
        where T : struct
    {
        Assert.AreEqual(check, algorithm.ComputeChecksum(data), "Checksum computation failed.");

        int n = data.Length;

        int m = n / 2 + 1;
        var iterator = algorithm.CreateIterator();
        iterator.ComputeBlock(data.AsSpan(0, m));
        iterator.ComputeBlock(data.AsSpan(m, n - m));
        Assert.AreEqual(check, iterator.ComputeFinal(), "Iterator checksum computation failed.");

        m = n / 2 - 2;
        iterator.ComputeBlock(data.AsSpan(0, m));
        iterator.ComputeBlock(data.AsSpan(m, n - m));
        Assert.AreEqual(check, iterator.ComputeFinal(), "Reused iterator checksum computation failed.");

        m = n / 2;
        iterator.ComputeBlock(data.AsSpan(0, m));
        iterator.Reset();

        --m;
        iterator.ComputeBlock(data.AsSpan(0, m));
        iterator.ComputeBlock(data.AsSpan(m, n - m));
        Assert.AreEqual(check, iterator.ComputeFinal(), "Reset iterator checksum computation failed.");

        var ms = new MemoryStream(data, false);
        Assert.AreEqual(check, algorithm.ComputeChecksum(ms), "Stream checksum computation failed.");

        ms.Position = 0;
        var checksum = TaskBridge.Execute(() => algorithm.ComputeChecksumAsync(ms));
        Assert.AreEqual(check, checksum, "Asynchronous stream checksum computation failed.");
    }
}
