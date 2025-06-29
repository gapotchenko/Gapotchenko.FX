// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("event")]
[TestCategory("reset")]
public abstract class IResetEventTests
{
    protected virtual IResetEvent CreateResetEvent() => CreateResetEvent(false);

    protected abstract IResetEvent CreateResetEvent(bool initialState);

    protected abstract bool IsAutoReset { get; }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void IResetEvent_Constructor()
    {
        var e = CreateResetEvent();
        Assert.AreEqual(IsAutoReset, e.IsAutoReset);
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void IResetEvent_Constructor_InitialState(bool initialState)
    {
        var e = CreateResetEvent(initialState);
        Assert.AreEqual(IsAutoReset, e.IsAutoReset);
        Assert.AreEqual(initialState, e.IsSet);
    }

    [TestMethod]
    public void IResetEvent_Set()
    {
        var e = CreateResetEvent();
        e.Set();
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    public void IResetEvent_Reset()
    {
        var e = CreateResetEvent(true);
        e.Reset();
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public void IResetEvent_SetReset()
    {
        var e = CreateResetEvent();
        e.Set();
        Assert.IsTrue(e.IsSet);
        e.Reset();
        Assert.IsFalse(e.IsSet);
    }

    [TestMethod]
    public void IResetEvent_ResetSet()
    {
        var e = CreateResetEvent(true);
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
    public void IResetEvent_Wait_Immediate(int timeout, bool initialState)
    {
        var e = CreateResetEvent(initialState);
        Assert.AreEqual(initialState, e.Wait(timeout));

        if (IsAutoReset)
            e = CreateResetEvent(initialState);
        Assert.AreEqual(initialState, e.Wait(timeout, CancellationToken.None));
    }

    [TestMethod]
    public void IResetEvent_Wait_AutoReset()
    {
        var e = CreateResetEvent(true);
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
    public void IResetEvent_Wait_DoubleSetAutoReset()
    {
        var e = CreateResetEvent();
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
    public void IResetEvent_Wait_CanceledAutoReset()
    {
        var e = CreateResetEvent(true);
        Assert.ThrowsExactly<TaskCanceledException>(() => e.Wait(0, new CancellationToken(true)));
        Assert.IsTrue(e.IsSet);
    }

    [TestMethod]
    public void IResetEvent_Scenario_A1_Sync()
    {
        var e1 = CreateResetEvent();
        var e2 = CreateResetEvent();
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

        var (tp1, tp2) = IsAutoReset ? (10, 200) : (12, 241);

        var task = TaskBridge.ExecuteAsync(TaskEntry);
        e1.Wait();
        if (e1.Wait(0))
            trace += 2;
        Assert.AreEqual(tp1, trace);
        e2.Set();

        TaskBridge.Execute(task);
        Assert.AreEqual(tp2, trace);
    }
}
