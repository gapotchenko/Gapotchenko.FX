using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace Gapotchenko.FX.Data.Checksum.Tests.Bench
{
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

            var iterator = algorithm.CreateIterator();
            iterator.ComputeBlock(data.AsSpan(0, 5));
            iterator.ComputeBlock(data.AsSpan(5, 4));
            Assert.AreEqual(check, iterator.ComputeFinal(), "Iterator checksum computation failed.");

            iterator.ComputeBlock(data.AsSpan(0, 2));
            iterator.ComputeBlock(data.AsSpan(2, 7));
            Assert.AreEqual(check, iterator.ComputeFinal(), "Reused iterator checksum computation failed.");

            iterator.ComputeBlock(data.AsSpan(0, 5));
            iterator.Reset();
            iterator.ComputeBlock(data.AsSpan(0, 3));
            iterator.ComputeBlock(data.AsSpan(3, 6));
            Assert.AreEqual(check, iterator.ComputeFinal(), "Reset iterator checksum computation failed.");

            var ms = new MemoryStream(data, false);
            Assert.AreEqual(check, algorithm.ComputeChecksum(ms), "Stream checksum computation failed.");

            ms.Position = 0;
            var checksum = TaskBridge.Execute(() => algorithm.ComputeChecksumAsync(ms));
            Assert.AreEqual(check, checksum, "Asynchronous stream checksum computation failed.");
        }
    }
}
