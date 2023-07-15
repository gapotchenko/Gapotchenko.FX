// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public sealed class AsyncManualResetEventTests
{
    [TestMethod]
    public void AsyncManualResetEvent_Constructor()
    {
        var e = new AsyncManualResetEvent();
        Assert.IsFalse(((IAsyncResetEvent)e).IsAutoReset);
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void AsyncManualResetEvent_Constructor_InitialState(bool initialState)
    {
        var e = new AsyncManualResetEvent(initialState);
        Assert.AreEqual(initialState, e.IsSet);
    }

    [TestMethod]
    public void AsyncManualResetEvent_Set()
    {
        var e = new AsyncManualResetEvent();
        e.Set();
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    public void AsyncManualResetEvent_Reset()
    {
        var e = new AsyncManualResetEvent(true);
        e.Reset();
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public void AsyncManualResetEvent_SetReset()
    {
        var e = new AsyncManualResetEvent();
        e.Set();
        Assert.IsTrue(e.IsSet);
        e.Reset();
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public void AsyncManualResetEvent_ResetSet()
    {
        var e = new AsyncManualResetEvent(true);
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
    public void AsyncManualResetEvent_Wait_Immediate(int timeout, bool initialState)
    {
        var e = new AsyncManualResetEvent(initialState);
        Assert.AreEqual(initialState, e.Wait(timeout));
        Assert.AreEqual(initialState, e.Wait(timeout, CancellationToken.None));
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(10, false)]
    [DataRow(10, true)]
    public async Task AsyncManualResetEvent_WaitAsync_Immediate(int timeout, bool initialState)
    {
        var e = new AsyncManualResetEvent(initialState);
        var t1 = e.WaitAsync(timeout);
        var t2 = e.WaitAsync(timeout, CancellationToken.None);

        Assert.AreEqual(initialState, await t1);
        Assert.AreEqual(initialState, await t2);
    }

    [TestMethod]
    public void AsyncManualResetEvent_Wait_ManualReset()
    {
        var e = new AsyncManualResetEvent(true);
        Assert.IsTrue(e.Wait(0));
        Assert.IsTrue(e.Wait(0));
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    public async Task AsyncManualResetEvent_WaitAsync_ManualReset()
    {
        var e = new AsyncManualResetEvent(true);
        Assert.IsTrue(await e.WaitAsync(0));
        Assert.IsTrue(await e.WaitAsync(0));
        Assert.IsTrue(e.IsSet);
    }
}
