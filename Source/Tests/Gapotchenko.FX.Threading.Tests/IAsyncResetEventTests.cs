// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("async")]
[TestCategory("event")]
[TestCategory("reset")]
public abstract class IAsyncResetEventTests
{
    protected virtual IAsyncResetEvent CreateAsyncResetEvent() => CreateAsyncResetEvent(false);

    protected abstract IAsyncResetEvent CreateAsyncResetEvent(bool initialState);

    protected abstract bool IsAutoReset { get; }

    [TestMethod]
    public void IAsyncResetEvent_Constructor()
    {
        var e = CreateAsyncResetEvent();
        Assert.AreEqual(IsAutoReset, e.IsAutoReset);
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void IAsyncResetEvent_Constructor_InitialState(bool initialState)
    {
        var e = CreateAsyncResetEvent(initialState);
        Assert.AreEqual(IsAutoReset, e.IsAutoReset);
        Assert.AreEqual(initialState, e.IsSet);
    }

    [TestMethod]
    public void IAsyncResetEvent_Set()
    {
        var e = CreateAsyncResetEvent();
        e.Set();
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    public void IAsyncResetEvent_Reset()
    {
        var e = CreateAsyncResetEvent(true);
        e.Reset();
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public void IAsyncResetEvent_SetReset()
    {
        var e = CreateAsyncResetEvent();
        e.Set();
        Assert.IsTrue(e.IsSet);
        e.Reset();
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public void IAsyncResetEvent_ResetSet()
    {
        var e = CreateAsyncResetEvent(true);
        e.Reset();
        Assert.IsFalse(e.IsSet);
        e.Set();
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(10, false)]
    [DataRow(10, true)]
    public void IAsyncResetEvent_Wait_Immediate(int timeout, bool initialState)
    {
        var e = CreateAsyncResetEvent(initialState);
        Assert.AreEqual(initialState, e.Wait(timeout));

        if (IsAutoReset)
            e = CreateAsyncResetEvent(initialState);
        Assert.AreEqual(initialState, e.Wait(timeout, CancellationToken.None));
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(10, false)]
    [DataRow(10, true)]
    public async Task IAsyncResetEvent_WaitAsync_Immediate(int timeout, bool initialState)
    {
        var e = CreateAsyncResetEvent(initialState);
        var t1 = e.WaitAsync(timeout);

        if (IsAutoReset)
            e = CreateAsyncResetEvent(initialState);
        var t2 = e.WaitAsync(timeout, CancellationToken.None);

        Assert.AreEqual(initialState, await t1);
        Assert.AreEqual(initialState, await t2);
    }

    [TestMethod]
    public void IAsyncResetEvent_Wait_AutoReset()
    {
        var e = CreateAsyncResetEvent(true);
        Assert.IsTrue(e.Wait(0));
        if (IsAutoReset)
        {
            Assert.IsFalse(e.IsSet);
            Assert.IsFalse(e.Wait(0));
        }
        else
        {
            Assert.IsTrue(e.Wait(0));
            Assert.IsTrue(e.IsSet);
        }
    }

    [TestMethod]
    public async Task IAsyncResetEvent_WaitAsync_AutoReset()
    {
        var e = CreateAsyncResetEvent(true);
        Assert.IsTrue(await e.WaitAsync(0));
        if (IsAutoReset)
        {
            Assert.IsFalse(e.IsSet);
            Assert.IsFalse(await e.WaitAsync(0));
        }
        else
        {
            Assert.IsTrue(await e.WaitAsync(0));
            Assert.IsTrue(e.IsSet);
        }
    }

    [TestMethod]
    public void IAsyncResetEvent_Wait_DoubleSetAutoReset()
    {
        var e = CreateAsyncResetEvent();
        e.Set();
        e.Set();
        Assert.IsTrue(e.Wait(0));
        if (IsAutoReset)
        {
            Assert.IsFalse(e.IsSet);
            Assert.IsFalse(e.Wait(0));
            Assert.IsFalse(e.IsSet);
        }
    }

    [TestMethod]
    public async Task IAsyncResetEvent_WaitAsync_DoubleSetAutoReset()
    {
        var e = CreateAsyncResetEvent();
        e.Set();
        e.Set();
        Assert.IsTrue(await e.WaitAsync(0));
        if (IsAutoReset)
        {
            Assert.IsFalse(e.IsSet);
            Assert.IsFalse(await e.WaitAsync(0));
            Assert.IsFalse(e.IsSet);
        }
    }

    [TestMethod]
    public void IAsyncResetEvent_Wait_CanceledAutoReset()
    {
        var e = CreateAsyncResetEvent(true);
        Assert.ThrowsException<TaskCanceledException>(() => e.Wait(0, new CancellationToken(true)));
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    public async Task IAsyncResetEvent_WaitAsync_CanceledAutoReset()
    {
        var e = CreateAsyncResetEvent(true);
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => e.WaitAsync(0, new CancellationToken(true)));
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    public void IAsyncResetEvent_Scenario_A1_Sync()
    {
        var e1 = CreateAsyncResetEvent();
        var e2 = CreateAsyncResetEvent();
        int trace = 0;

        void TaskEntry()
        {
            trace = 10;
            e1.Set();
            if (e2.Wait(Timeout.Infinite))
                trace *= 20;
            if (e2.Wait(0))
                ++trace;
        }

        var (tp1, tp2) = AsyncResetEvent_Scenario_A1_GetTracePoints();

        var task = TaskBridge.ExecuteAsync(TaskEntry);
        e1.Wait();
        if (e1.Wait(0))
            trace += 2;
        Assert.AreEqual(tp1, trace);
        e2.Set();

        TaskBridge.Execute(task);
        Assert.AreEqual(tp2, trace);
    }

    [TestMethod]
    public async Task IAsyncResetEvent_Scenario_A1_Async()
    {
        var e1 = CreateAsyncResetEvent();
        var e2 = CreateAsyncResetEvent();
        int trace = 0;

        async Task TaskEntry()
        {
            trace = 10;
            e1.Set();
            if (await e2.WaitAsync(Timeout.Infinite))
                trace *= 20;
            if (await e2.WaitAsync(0))
                ++trace;
        }

        var (tp1, tp2) = AsyncResetEvent_Scenario_A1_GetTracePoints();

        var task = TaskEntry();
        await e1.WaitAsync();
        if (await e1.WaitAsync(0))
            trace += 2;
        Assert.AreEqual(tp1, trace);
        e2.Set();

        await task;
        Assert.AreEqual(tp2, trace);
    }

    (int, int) AsyncResetEvent_Scenario_A1_GetTracePoints()
    {
        if (IsAutoReset)
            return (10, 200);
        else
            return (12, 241);
    }
}
