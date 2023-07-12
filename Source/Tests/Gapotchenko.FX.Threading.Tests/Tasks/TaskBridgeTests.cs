// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Tests.Tasks;

[TestClass]
public sealed class TaskBridgeTests
{
    #region Settings

#pragma warning disable IDE1006 // Naming Styles

    static readonly TimeSpan TestData_CancellationDelay = TimeSpan.FromMilliseconds(200);
    static readonly TimeSpan TestData_PositiveTimeout = TimeSpan.FromMilliseconds(30000);
    static readonly TimeSpan TestData_NegativeTimeout = TimeSpan.FromMilliseconds(200);

#pragma warning restore IDE1006 // Naming Styles

    #endregion

    [TestMethod]
    public void TaskBridge_ThreadAffinity()
    {
        var map = new Dictionary<int, int>();

        static async Task Run(Dictionary<int, int> mapArg)
        {
            for (int i = 0; i < 10000; i++)
            {
                int tid = Environment.CurrentManagedThreadId;
                mapArg[tid] = mapArg.TryGetValue(tid, out var count) ? count + 1 : 1;
                await Task.Yield();
            }
        }

        TaskBridge.Execute(() => Run(map));

        Assert.AreEqual(1, map.Count);
        Assert.AreEqual(10000, map.Single().Value);
    }

    [TestMethod]
    public void TaskBridge_CancelWait()
    {
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        using var ev = new ManualResetEventSlim();

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
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        using var ev = new ManualResetEventSlim();

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

    [TestMethod]
    public async Task TaskBridge_CancelSync()
    {
        using var startSemaphore = new SemaphoreSlim(0, 1);
        using var barrierSemaphore = new SemaphoreSlim(0, 1);
        using var exitSemaphore = new SemaphoreSlim(0, 1);
        Exception? exitException = null;

        void SyncProcedure()
        {
            try
            {
                startSemaphore.Release();
                barrierSemaphore.Wait();
            }
            catch (Exception e)
            {
                exitException = e;
                exitSemaphore.Release();
                throw;
            }
            throw new UnreachableException();
        }

        try
        {
            using var cts = new CancellationTokenSource();

            var task = TaskBridge.ExecuteAsync(SyncProcedure, cts.Token);
            await startSemaphore.WaitAsync();

            cts.CancelAfter(TestData_CancellationDelay);

            var cancellationException = await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => task.WaitAsync(TestData_PositiveTimeout));
            Assert.AreEqual(task, cancellationException.Task);
        }
        finally
        {
            // Allow a synchronous thread to exit if it hasn't been canceled for some reason.
            barrierSemaphore.Release();
        }

        Assert.IsTrue(await exitSemaphore.WaitAsync(TestData_PositiveTimeout));

#if NETFRAMEWORK
        Assert.IsInstanceOfType<ThreadAbortException>(exitException);
#elif NETCOREAPP
        Assert.IsInstanceOfType<ThreadInterruptedException>(exitException);
#endif
    }
}
