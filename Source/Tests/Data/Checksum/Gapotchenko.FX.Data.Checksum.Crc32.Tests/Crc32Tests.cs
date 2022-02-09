using Gapotchenko.FX.Data.Checksum.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Data.Checksum.Tests
{
    [TestClass]
    public class Crc32Tests
    {
        static void Check19(IChecksumAlgorithm<uint> algorithm, uint check) => ChecksumTestBench.Check(algorithm, ChecksumTestBench.TV19, check);

    }
}
