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
[TestCategory("task")]
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

        Assert.ThrowsExactly<OperationCanceledException>(() => TaskBridge.Execute(task));
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

        Assert.ThrowsExactly<AggregateException>(() => TaskBridge.Execute(task));
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

        var exception = Assert.ThrowsExactly<Exception>(() => TaskBridge.Execute(F));
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

            var cancellationException = await Assert.ThrowsExactlyAsync<TaskCanceledException>(() => task.WaitAsync(TestData_PositiveTimeout));
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

    [TestMethod]
    public void TaskBridge_TaskCompletion()
    {
        int trace = 0;
        const string exceptionMessage = "Expected";

        async Task RunAsync()
        {
            try
            {
                try
                {
                    ++trace;
                    await Task.Yield();
                    ++trace;
                    if (trace == 2)
                        throw new Exception(exceptionMessage);
                    ++trace;
                }
                catch
                {
                    await Task.Yield();
                    trace += 10;
                    await Task.Yield();
                    trace += 10;
                    throw;
                }
                finally
                {
                    trace += 100;
                }
            }
            finally
            {
                trace += 1000;
                await Task.Yield();
                trace += 1000;
            }
        }

        var exception = Assert.ThrowsExactly<Exception>(() => TaskBridge.Execute(RunAsync));
        Assert.AreEqual(exceptionMessage, exception.Message);

        Assert.AreEqual(2122, trace);
    }

    [TestMethod]
    [Timeout(2 * 60 * 1000)]
    public async Task TaskBridge_TaskCompletionOnSyncCancel()
    {
        int trace = 0;
        var flag = new AsyncManualResetEvent();

        async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    ++trace;
                    await Task.Yield();
                    ++trace;
                    flag.Set();
                    await Task.Delay(Timeout.Infinite, cancellationToken);
                    ++trace;
                }
                catch
                {
                    await Task.Yield();
                    trace += 10;
                    await Task.Yield();
                    trace += 10;
                    throw;
                }
                finally
                {
                    trace += 100;
                }
            }
            finally
            {
                trace += 1000;
                await Task.Yield();
                trace += 1000;
            }
        }

        var cts = new CancellationTokenSource();

        async Task ControlTask()
        {
            await flag.WaitAsync();
            cts.Cancel();
        }

        var controlTask = ControlTask();
        await Assert.ThrowsExactlyAsync<TaskCanceledException>(
            () => TaskBridge.ExecuteAsync(
                () => TaskBridge.Execute(RunAsync),
                cts.Token));

        await controlTask;

        Assert.AreEqual(2122, trace);
    }

    [TestMethod]
    [Timeout(2 * 60 * 1000)]
    public async Task TaskBridge_TaskCompletionOnSyncAbort()
    {
        int trace = 0;
        var flag1 = new AsyncManualResetEvent();
        var flag2 = new AsyncManualResetEvent();

        async Task RunAsync()
        {
            try
            {
                ++trace;
                await Task.Yield();
                ++trace;
                flag1.Set();
                await flag2.WaitAsync();
                ++trace;
            }
            finally
            {
                trace += 100;
                await Task.Yield();
                trace += 100;
            }
        }

        var cts = new CancellationTokenSource();

        async Task ControlTask()
        {
            await flag1.WaitAsync();
            cts.Cancel();
        }

        Task? pendingTask = null;

        var controlTask = ControlTask();
        await Assert.ThrowsExactlyAsync<TaskCanceledException>(
            () => TaskBridge.ExecuteAsync(
                () => TaskBridge.Execute(() => pendingTask = RunAsync()),
                cts.Token));

        await controlTask;

        Assert.AreEqual(2, trace);

        flag2.Set();

        Assert.IsNotNull(pendingTask);
        await pendingTask;

        Assert.AreEqual(203, trace);
    }
}
