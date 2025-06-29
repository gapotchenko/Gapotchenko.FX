// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("async")]
public abstract class IAsyncResetEventTests : IResetEventTests
{
    protected virtual IAsyncResetEvent CreateAsyncResetEvent() => CreateAsyncResetEvent(false);

    protected abstract IAsyncResetEvent CreateAsyncResetEvent(bool initialState);

    protected sealed override IResetEvent CreateResetEvent() => CreateAsyncResetEvent();

    protected sealed override IResetEvent CreateResetEvent(bool initialState) => CreateAsyncResetEvent(initialState);

    // ----------------------------------------------------------------------

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
    public async Task IAsyncResetEvent_WaitAsync_CanceledAutoReset()
    {
        var e = CreateAsyncResetEvent(true);
        await Assert.ThrowsExactlyAsync<TaskCanceledException>(() => e.WaitAsync(0, new CancellationToken(true)));
        Assert.IsTrue(e.IsSet);
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

        var (tp1, tp2) = IsAutoReset ? (10, 200) : (12, 241);

        var task = TaskEntry();
        await e1.WaitAsync();
        if (await e1.WaitAsync(0))
            trace += 2;
        Assert.AreEqual(tp1, trace);
        e2.Set();

        await task;
        Assert.AreEqual(tp2, trace);
    }
}
