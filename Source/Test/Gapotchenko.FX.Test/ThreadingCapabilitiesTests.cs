using Gapotchenko.FX.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gapotchenko.FX.Test
{
    [TestClass]
    public class ThreadingCapabilitiesTests
    {
        [TestMethod]
        public void ThreadingCapabilities_LogicalProcessorCount()
        {
            Assert.AreEqual(Environment.ProcessorCount, ThreadingCapabilities.LogicalProcessorCount);
        }
    }
}
