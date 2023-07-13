using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public sealed class AsyncAutoResetEventTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void AsyncAutoResetEvent_Constructor(bool initialState)
    {
        var e = new AsyncAutoResetEvent(initialState);
        Assert.IsTrue(((IAsyncResetEvent)e).IsAutoReset);
        Assert.AreEqual(initialState, e.IsSet);
    }

    [TestMethod]
    public void AsyncAutoResetEvent_Set()
    {
        var e = new AsyncAutoResetEvent(false);
        e.Set();
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    public void AsyncAutoResetEvent_Reset()
    {
        var e = new AsyncAutoResetEvent(true);
        e.Reset();
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public void AsyncAutoResetEvent_SetReset()
    {
        var e = new AsyncAutoResetEvent(false);
        e.Set();
        Assert.IsTrue(e.IsSet);
        e.Reset();
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public void AsyncAutoResetEvent_ResetSet()
    {
        var e = new AsyncAutoResetEvent(true);
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
    public void AsyncAutoResetEvent_WaitOne_Immediate(int timeout, bool initialState)
    {
        var e = new AsyncAutoResetEvent(initialState);
        Assert.AreEqual(initialState, e.WaitOne(timeout));

        e = new AsyncAutoResetEvent(initialState);
        Assert.AreEqual(initialState, e.WaitOne(timeout, CancellationToken.None));
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(10, false)]
    [DataRow(10, true)]
    public async Task AsyncAutoResetEvent_WaitOneAsync_Immediate(int timeout, bool initialState)
    {
        var e = new AsyncAutoResetEvent(initialState);
        var t1 = e.WaitOneAsync(timeout);

        e = new AsyncAutoResetEvent(initialState);
        var t2 = e.WaitOneAsync(timeout, CancellationToken.None);

        Assert.AreEqual(initialState, await t1);
        Assert.AreEqual(initialState, await t2);
    }

    [TestMethod]
    public void AsyncAutoResetEvent_WaitOne_AutoReset()
    {
        var e = new AsyncAutoResetEvent(true);
        Assert.IsTrue(e.WaitOne(0));
        Assert.IsFalse(e.IsSet);
        Assert.IsFalse(e.WaitOne(0));
    }

    [TestMethod]
    public async Task AsyncAutoResetEvent_WaitOneAsync_AutoReset()
    {
        var e = new AsyncAutoResetEvent(true);
        Assert.IsTrue(await e.WaitOneAsync(0));
        Assert.IsFalse(e.IsSet);
        Assert.IsFalse(await e.WaitOneAsync(0));
    }

    [TestMethod]
    public void AsyncAutoResetEvent_WaitOne_DoubleSetAutoReset()
    {
        var e = new AsyncAutoResetEvent(false);
        e.Set();
        e.Set();
        Assert.IsTrue(e.WaitOne(0));
        Assert.IsFalse(e.IsSet);
        Assert.IsFalse(e.WaitOne(0));
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public async Task AsyncAutoResetEvent_WaitOneAsync_DoubleSetAutoReset()
    {
        var e = new AsyncAutoResetEvent(false);
        e.Set();
        e.Set();
        Assert.IsTrue(await e.WaitOneAsync(0));
        Assert.IsFalse(e.IsSet);
        Assert.IsFalse(await e.WaitOneAsync(0));
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public void AsyncAutoResetEvent_WaitOne_CanceledAutoReset()
    {
        var e = new AsyncAutoResetEvent(true);
        Assert.ThrowsException<TaskCanceledException>(() => e.WaitOne(0, new CancellationToken(true)));
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    public async Task AsyncAutoResetEvent_WaitOneAsync_CanceledAutoReset()
    {
        var e = new AsyncAutoResetEvent(true);
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => e.WaitOneAsync(0, new CancellationToken(true)));
        Assert.IsTrue(e.IsSet);
    }
}
