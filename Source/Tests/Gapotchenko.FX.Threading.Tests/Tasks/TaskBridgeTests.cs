using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Threading.Tests.Tasks;

[TestClass]
public class TaskBridgeTests
{
    [TestMethod]
    public void TaskBridge_ThreadAffinity()
    {
        var map = new Dictionary<int, int>();

        static async Task _ThreadAffinityChecker(Dictionary<int, int> mapArg)
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

    [TestMethod]
    public void TaskBridge_CancelWait()
    {
        var cts = new CancellationTokenSource();
        var ct = cts.Token;

        var ev = new ManualResetEventSlim();

        var task = Task.Factory.StartNew(
            () =>
            {
                for (; ; )
                {
                    ct.ThrowIfCancellationRequested();
                    Thread.Sleep(10);
                    ev.Set();
                }
            },
            TaskCreationOptions.LongRunning);

        ev.Wait();
        cts.Cancel();

        Assert.ThrowsException<OperationCanceledException>(() => TaskBridge.Execute(task));
    }

    [TestMethod]
    public void TaskBridge_CancelWaitAggregate()
    {
        var cts = new CancellationTokenSource();
        var ct = cts.Token;

        var ev = new ManualResetEventSlim();

        var task = Task.Factory.StartNew(
            () =>
            {
                var thread = new Action(() =>
                {
                    for (; ; )
                    {
                        ct.ThrowIfCancellationRequested();
                        Thread.Sleep(10);
                        ev.Set();
                    }
                });

                Parallel.Invoke(thread, thread);
            },
            TaskCreationOptions.LongRunning);

        ev.Wait();
        cts.Cancel();

        Assert.ThrowsException<AggregateException>(() => TaskBridge.Execute(task));
    }

    [TestMethod]
    public void TaskBridge_AsyncString()
    {
        static async Task<string> F(string s)
        {
            await Task.Yield();
            return s;
        }

        Assert.AreEqual("abc", TaskBridge.Execute(F("abc")));
        Assert.AreEqual("def", TaskBridge.Execute(() => F("def")));
    }

    [TestMethod]
    public void TaskBridge_AsyncSequence()
    {
        if (!Environment.UserInteractive)
            Assert.Inconclusive();

        static async Task<string> F(string s)
        {
            int count = 0;

            await Task.Delay(1);
            ++count;

            await Task.Delay(1);
            ++count;

            await Task.Delay(1);
            ++count;

            return s + count;
        }

        Assert.AreEqual("abc3", TaskBridge.Execute(() => F("abc")));
        Assert.AreEqual("def3", TaskBridge.Execute(F("def")));
    }

    [TestMethod]
    public void TaskBridge_AsyncInt32Exception()
    {
        static async Task<int> F()
        {
            await Task.Yield();
            throw new Exception("Expected exception.");
        }

        var exception = Assert.ThrowsException<Exception>(() => TaskBridge.Execute(F));
        Assert.AreEqual(typeof(Exception), exception.GetType());
        Assert.AreEqual("Expected exception.", exception.Message);
    }

    [TestMethod]
    public void TaskBridge_SyncContext_Send()
    {
        int value1 = 0, value2 = 0;

        TaskBridge.Execute(async () =>
        {
            Assert.IsNotNull(SynchronizationContext.Current);
            SynchronizationContext.Current!.Send(_ => { value1 = 100; }, null);
            await Task.Yield();
            SynchronizationContext.Current.Send(_ => { value2 = 200; }, null);
        });

        Assert.AreEqual(100, value1);
        Assert.AreEqual(200, value2);
    }
}
