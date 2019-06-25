using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Threading.Test.Tasks
{
    [TestClass]
    public class TaskBridgeTests
    {
        [TestMethod]
        public void TaskBridge_ThreadAffinity()
        {
            var map = new Dictionary<int, int>();

            async Task _ThreadAffinityChecker(Dictionary<int, int> mapArg)
            {
                for (int i = 0; i < 10000; i++)
                {
                    int tid = Thread.CurrentThread.ManagedThreadId;
                    mapArg[tid] = mapArg.TryGetValue(tid, out var count) ? count + 1 : 1;
                    await Task.Yield();
                }
            }

            TaskBridge.Execute(() => _ThreadAffinityChecker(map));

            Assert.AreEqual(1, map.Count);
            Assert.AreEqual(10000, map.Single().Value);
        }
    }
}
