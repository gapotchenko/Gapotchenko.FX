// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Math.Combinatorics;
using Gapotchenko.FX.Threading.Tasks;
using Gapotchenko.FX.Threading.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("condition-variable")]
public abstract class IConditionVariableTests
{
    protected const int IConditionVariable_TestTimeout = 50000;
    protected static readonly TimeSpan IConditionVariable_PositiveTimeout = TimeSpan.FromMilliseconds(30000);
    protected static readonly TimeSpan IConditionVariable_NegativeTimeout = TimeSpan.FromMilliseconds(200);

    // ----------------------------------------------------------------------

    protected abstract IConditionVariable CreateConditionVariable();

    protected abstract ILockable GetLockable(IConditionVariable conditionVariable);

    protected abstract bool IsRecursive { get; }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void IConditionVariable_RecursionDeclaration()
    {
        var cv = CreateConditionVariable();
        var lockable = GetLockable(cv);

        Assert.AreEqual(IsRecursive, lockable.IsRecursive);
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void IConditionVariable_Notify_ThrowsWhenUnlocked()
    {
        var cv = CreateConditionVariable();

        Assert.ThrowsException<SynchronizationLockException>(cv.Notify);
    }

    [TestMethod]
    public void IConditionVariable_NotifyAll_ThrowsWhenUnlocked()
    {
        var cv = CreateConditionVariable();

        Assert.ThrowsException<SynchronizationLockException>(cv.NotifyAll);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    [DataRow(Timeout.Infinite)]
    public void IConditionVariable_Wait_ThrowsWhenUnlocked(int millisecondsTimeout)
    {
        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        var cv = CreateConditionVariable();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
            Assert.ThrowsException<SynchronizationLockException>(() => waitFunc(timeout));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void IConditionVariable_Wait_LocksOnExit(int millisecondsTimeout)
    {
        foreach (var recursionLevel in EnumerateRecursionLevels())
            Run(recursionLevel);

        void Run(int recursionLevel)
        {
            var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

            var cv = CreateConditionVariable();
            var lockable = GetLockable(cv);
            using var lockScope = lockable.EnterScopeRecursively(recursionLevel);

            foreach (var waitFunc in EnumerateWaitFunctions(cv))
            {
                Assert.IsFalse(waitFunc(timeout));
                Assert.IsTrue(LockableHelper.IsLockHeld(lockable));
            }
        }
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void IConditionVariable_Wait_NoDeferredNotification(int millisecondsTimeout)
    {
        foreach (var recursionLevel in EnumerateRecursionLevels())
            Run(recursionLevel);

        void Run(int recursionLevel)
        {
            var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

            var cv = CreateConditionVariable();
            using var lockScope = GetLockable(cv).EnterScopeRecursively(recursionLevel);

            foreach (var waitFunc in EnumerateWaitFunctions(cv))
            {
                cv.NotifyAll();
                Assert.IsFalse(waitFunc(timeout));
            }
        }
    }

    [TestMethod]
    public void IConditionVariable_Wait_DoesNotCompleteWithoutNotify()
    {
        foreach (var recursionLevel in EnumerateRecursionLevels())
            Run(recursionLevel);

        void Run(int recursionLevel)
        {
            var cv = CreateConditionVariable();
            using var lockScope = GetLockable(cv).EnterScopeRecursively(recursionLevel);

            foreach (var waitFunc in EnumerateWaitFunctions(cv))
                Assert.IsFalse(waitFunc(IConditionVariable_NegativeTimeout));
        }
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [Timeout(IConditionVariable_TestTimeout)]
    public void IAsyncConditionVariable_Wait_CompletesAfterNotify()
    {
        var cv = CreateConditionVariable();

        foreach (var i in CartesianProduct.Of(
            EnumerateWaitFunctions(cv),
            EnumerateRecursionLevels(),
            ValueTuple.Create))
        {
            Run(i.Item1, i.Item2);
        }

        void Run(Func<TimeSpan, bool> waitFunc, int recursionLevel)
        {
            var lockable = GetLockable(cv);
            using var lockScope = lockable.EnterScope();

            var timeout = IConditionVariable_PositiveTimeout;

            #region Notify

            void NotifyTask()
            {
                using (lockable.EnterScope())
                    cv.Notify();
            }

            Task notificationTask;
            using (ExecutionContext.SuppressFlow())
                notificationTask = TaskBridge.ExecuteAsync(NotifyTask);

            Assert.IsTrue(waitFunc(timeout));
            TaskBridge.Execute(notificationTask);

            #endregion

            #region NotifyAll

            void NotifyAllTask()
            {
                using (lockable.EnterScope())
                    cv.NotifyAll();
            }

            using (ExecutionContext.SuppressFlow())
                notificationTask = TaskBridge.ExecuteAsync(NotifyAllTask);

            Assert.IsTrue(waitFunc(timeout));
            TaskBridge.Execute(notificationTask);

            #endregion
        }
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [Timeout(IConditionVariable_TestTimeout)]
    public void IConditionVariable_Wait_OneCompletesAfterNotify()
    {
        var cv = CreateConditionVariable();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
            RunCore(waitFunc);

        void RunCore(Func<TimeSpan, bool> waitFunc)
        {
            var lockable = GetLockable(cv);
            using var lockAcquiredEvent1 = new AutoResetEvent(false);
            using var lockAcquiredEvent2 = new AutoResetEvent(false);

            bool WaitTask(TimeSpan timeout, AutoResetEvent lockAcquiredEvent)
            {
                using (lockable.EnterScope())
                {
                    lockAcquiredEvent.Set();
                    return waitFunc(timeout);
                }
            }

            Task<bool> waitTask1;
            using (ExecutionContext.SuppressFlow())
                waitTask1 = TaskBridge.ExecuteAsync(() => WaitTask(IConditionVariable_PositiveTimeout, lockAcquiredEvent1));

            Assert.IsTrue(lockAcquiredEvent1.WaitOne(IConditionVariable_PositiveTimeout));

            Task<bool> waitTask2;
            using (ExecutionContext.SuppressFlow())
                waitTask2 = TaskBridge.ExecuteAsync(() => WaitTask(IConditionVariable_NegativeTimeout, lockAcquiredEvent2));

            Assert.IsTrue(lockAcquiredEvent2.WaitOne(IConditionVariable_PositiveTimeout));

            void NotifyTask()
            {
                using (lockable.EnterScope())
                    cv.Notify();
            }

            Task notificationTask;
            using (ExecutionContext.SuppressFlow())
                notificationTask = TaskBridge.ExecuteAsync(NotifyTask);

            var waitResults = TaskBridge.Execute(Task.WhenAll(waitTask1, waitTask2));

            Assert.IsTrue(waitResults[0]);
            Assert.IsFalse(waitResults[1]);

            TaskBridge.Execute(notificationTask);
        }
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void IConditionVariable_Wait_AllCompleteAfterNotifyAll()
    {
        var cv = CreateConditionVariable();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
            RunCore(waitFunc);

        void RunCore(Func<TimeSpan, bool> waitFunc)
        {
            var lockable = GetLockable(cv);
            using var lockAcquiredEvent1 = new AutoResetEvent(false);
            using var lockAcquiredEvent2 = new AutoResetEvent(false);

            bool WaitTask(TimeSpan timeout, AutoResetEvent lockAcquiredEvent)
            {
                using (lockable.EnterScope())
                {
                    lockAcquiredEvent.Set();
                    return waitFunc(timeout);
                }
            }

            Task<bool> waitTask1;
            using (ExecutionContext.SuppressFlow())
                waitTask1 = TaskBridge.ExecuteAsync(() => WaitTask(IConditionVariable_PositiveTimeout, lockAcquiredEvent1));

            Task<bool> waitTask2;
            using (ExecutionContext.SuppressFlow())
                waitTask2 = TaskBridge.ExecuteAsync(() => WaitTask(IConditionVariable_PositiveTimeout, lockAcquiredEvent2));

            Assert.IsTrue(lockAcquiredEvent1.WaitOne(IConditionVariable_PositiveTimeout));
            Assert.IsTrue(lockAcquiredEvent2.WaitOne(IConditionVariable_PositiveTimeout));

            void NotifyTask()
            {
                using (lockable.EnterScope())
                    cv.NotifyAll();
            }

            Task notificationTask;
            using (ExecutionContext.SuppressFlow())
                notificationTask = TaskBridge.ExecuteAsync(NotifyTask);

            var waitResults = TaskBridge.Execute(Task.WhenAll(waitTask1, waitTask2));

            Assert.IsTrue(waitResults[0]);
            Assert.IsTrue(waitResults[1]);

            TaskBridge.Execute(notificationTask);
        }
    }

    // ----------------------------------------------------------------------

    #region Helpers

    IEnumerable<int> EnumerateRecursionLevels() => EnumerateRecursionLevels(IsRecursive);

    static IEnumerable<int> EnumerateRecursionLevels(bool isRecursive)
    {
        if (isRecursive)
            return [1, 2, 3];
        else
            return [1];
    }

    static IEnumerable<Func<TimeSpan, bool>> EnumerateWaitFunctions(IConditionVariable cv)
    {
        yield return timeout => cv.Wait(TimeoutHelper.GetMillisecondsTimeout(timeout));
        yield return timeout => cv.Wait(TimeoutHelper.GetMillisecondsTimeout(timeout), CancellationToken.None);
        yield return timeout => cv.Wait(timeout);
        yield return timeout => cv.Wait(timeout, CancellationToken.None);

        yield return
            timeout =>
            {
                if (timeout >= IConditionVariable_PositiveTimeout)
                {
                    cv.Wait();
                    return true;
                }
                else
                {
                    return cv.Wait(timeout);
                }
            };

        yield return
            timeout =>
            {
                if (timeout >= IConditionVariable_PositiveTimeout)
                {
                    cv.Wait(CancellationToken.None);
                    return true;
                }
                else
                {
                    return cv.Wait(timeout, CancellationToken.None);
                }
            };
    }

    #endregion
}
