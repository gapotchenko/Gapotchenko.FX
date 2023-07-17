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

    //protected abstract IAsyncMonitor GetAsyncMonitorFor(object obj);

    protected sealed override IAsyncConditionVariable CreateAsyncConditionVariable() => CreateAsyncMonitor();

    protected sealed override IAsyncLockable GetAsyncLockable(IAsyncConditionVariable conditionVariable) => (IAsyncMonitor)conditionVariable;

    [TestMethod]
    public void IAsyncMonitorTests_Constructor()
    {
        var monitor = CreateAsyncMonitor();
        Assert.AreEqual(monitor.IsRecursive, monitor is IAsyncRecursiveMonitor);
    }
}
