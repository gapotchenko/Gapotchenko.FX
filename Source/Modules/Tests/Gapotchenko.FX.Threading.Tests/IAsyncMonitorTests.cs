// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using System.Text;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("monitor")]
public abstract class IAsyncMonitorTests : IAsyncConditionVariableTests
{
    protected const int IAsyncMonitor_TestTimeout = 30000;

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

    #region For

    [TestMethod]
    public void IAsyncMonitor_For_ThrowsOnNull()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => GetAsyncMonitorFor(null!));
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

    #endregion

    #region Scenarios

    [TestMethod]
    [Timeout(IAsyncMonitor_TestTimeout)]
    public void IAsyncMonitor_Scenario_A1()
    {
        var monitor = CreateAsyncMonitor();
        var queue = new Queue<char>();
        var sb = new StringBuilder();
        const string text = "abcdef";

        async Task Worker()
        {
            for (; ; )
            {
                char c;
                using (await monitor.EnterScopeAsync())
                {
                    while (queue.Count == 0)
                        await monitor.WaitAsync();

                    c = queue.Dequeue();
                }

                if (c == default)
                    return;

                sb.Append(c);
            }
        }

        Task task;
        using (ExecutionContext.SuppressFlow())
            task = Task.Run(Worker);

        void Enqueue(char c)
        {
            using (monitor.EnterScope())
            {
                queue.Enqueue(c);
                monitor.Notify();
            }
        }

        foreach (var c in text)
            Enqueue(c);

        Enqueue(default);
        TaskBridge.Execute(task);

        Assert.AreEqual(text, sb.ToString());
    }

    #endregion
}
