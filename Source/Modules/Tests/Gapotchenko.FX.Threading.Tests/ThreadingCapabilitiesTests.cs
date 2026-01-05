// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public sealed class ThreadingCapabilitiesTests
{
    [TestMethod]
    public void ThreadingCapabilities_LogicalProcessorCount()
    {
        Assert.AreEqual(Environment.ProcessorCount, ThreadingCapabilities.LogicalProcessorCount);
    }
}
