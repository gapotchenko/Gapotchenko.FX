// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("monitor")]
public abstract class IAsyncMonitorTests : IAsyncConditionVariableTests
{
    protected abstract IAsyncMonitor CreateAsyncMonitor();

    protected abstract IAsyncMonitor GetAsyncMonitorFor(object obj);

    protected sealed override IAsyncConditionVariable CreateAsyncConditionVariable() => CreateAsyncMonitor();

    protected sealed override IAsyncLockable GetAsyncLockable(IAsyncConditionVariable conditionVariable) => (IAsyncMonitor)conditionVariable;

    [TestMethod]
    public void IAsyncMonitor_Constructor()
    {
        var monitor = CreateAsyncMonitor();
        Assert.AreEqual(monitor.IsRecursive, monitor is IAsyncRecursiveMonitor);
    }

    [TestMethod]
    public void IAsyncMonitor_For_ThrowsOnNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() => GetAsyncMonitorFor(null!));
    }

    [TestMethod]
    public void IAsyncMonitor_For_ReturnsExpectedType()
    {
        var obj = new object();
        var objMonitor = GetAsyncMonitorFor(obj);
        Assert.IsNotNull(objMonitor);

        var monitor = CreateAsyncMonitor();

        Assert.IsInstanceOfType(objMonitor, monitor.GetType());
        Assert.AreEqual(monitor.IsRecursive, objMonitor.IsRecursive);
    }

    [TestMethod]
    public void IAsyncMonitor_For_ReturnsSameMonitorForTheSameObject()
    {
        var obj = new object();

        var monitor1 = GetAsyncMonitorFor(obj);
        var monitor2 = GetAsyncMonitorFor(obj);

        Assert.AreSame(monitor1, monitor2);
    }

    [TestMethod]
    public void IAsyncMonitor_For_ReturnsDifferentMonitorsForDifferentObjects()
    {
        var monitor1 = GetAsyncMonitorFor(new object());
        var monitor2 = GetAsyncMonitorFor(new object());

        Assert.AreNotSame(monitor1, monitor2);
    }
}
