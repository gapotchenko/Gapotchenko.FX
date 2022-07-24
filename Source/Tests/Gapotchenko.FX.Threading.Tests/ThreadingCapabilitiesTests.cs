﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public class ThreadingCapabilitiesTests
{
    [TestMethod]
    public void ThreadingCapabilities_LogicalProcessorCount()
    {
        Assert.AreEqual(Environment.ProcessorCount, ThreadingCapabilities.LogicalProcessorCount);
    }
}
