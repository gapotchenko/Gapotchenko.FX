// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Gapotchenko.FX.Threading.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("async")]
[TestCategory("condition-variable")]
public abstract class IAsyncConditionVariableTests
{
    protected const int IAsyncConditionVariable_TestTimeout = 50000;
    protected static readonly TimeSpan IAsyncConditionVariable_PositiveTimeout = TimeSpan.FromMilliseconds(30000);
    protected static readonly TimeSpan IAsyncConditionVariable_NegativeTimeout = TimeSpan.FromMilliseconds(200);

    // ----------------------------------------------------------------------

    protected abstract IAsyncConditionVariable CreateAsyncConditionVariable();

    protected abstract IAsyncLockable GetAsyncLockable(IAsyncConditionVariable conditionVariable);

    // ----------------------------------------------------------------------

    [TestMethod]
    public void IAsyncConditionVariable_Notify_ThrowsWhenUnlocked()
    {
        var cv = CreateAsyncConditionVariable();

        Assert.ThrowsException<SynchronizationLockException>(cv.Notify);
    }

    [TestMethod]
    public void IAsyncConditionVariable_NotifyAll_ThrowsWhenUnlocked()
    {
        var cv = CreateAsyncConditionVariable();

        Assert.ThrowsException<SynchronizationLockException>(cv.NotifyAll);
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    [DataRow(Timeout.Infinite)]
    public void IAsyncConditionVariable_Wait_ThrowsWhenUnlocked(int millisecondsTimeout)
    {
        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
            Assert.ThrowsException<SynchronizationLockException>(() => waitFunc(timeout));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    [DataRow(Timeout.Infinite)]
    public async Task IAsyncConditionVariable_WaitAsync_ThrowsWhenUnlocked(int millisecondsTimeout)
    {
        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
            await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => waitFunc(timeout));
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void IAsyncConditionVariable_Wait_LocksOnExit(int millisecondsTimeout)
    {
        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        var cv = CreateAsyncConditionVariable();
        var lockable = GetAsyncLockable(cv);
        using var lockScope = lockable.EnterScope();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
        {
            Assert.IsFalse(waitFunc(timeout));
            Assert.IsTrue(AsyncLockableHelper.IsLockHeld(lockable));
        }
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public async Task IAsyncConditionVariable_WaitAsync_LocksOnExit(int millisecondsTimeout)
    {
        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        var cv = CreateAsyncConditionVariable();
        var lockable = GetAsyncLockable(cv);
        using var lockScope = await lockable.EnterScopeAsync();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
        {
            Assert.IsFalse(await waitFunc(timeout));
            Assert.IsTrue(AsyncLockableHelper.IsLockHeld(lockable));
        }
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void IAsyncConditionVariable_Wait_NoDeferredNotification(int millisecondsTimeout)
    {
        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        var cv = CreateAsyncConditionVariable();
        using var lockScope = GetAsyncLockable(cv).EnterScope();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
        {
            cv.NotifyAll();
            Assert.IsFalse(waitFunc(timeout));
        }
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public async Task IAsyncConditionVariable_WaitAsync_NoDeferredNotification(int millisecondsTimeout)
    {
        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        var cv = CreateAsyncConditionVariable();
        using var lockScope = await GetAsyncLockable(cv).EnterScopeAsync();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
        {
            cv.NotifyAll();
            Assert.IsFalse(await waitFunc(timeout));
        }
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void IAsyncConditionVariable_Wait_DoesNotCompleteWithoutNotify()
    {
        var cv = CreateAsyncConditionVariable();
        using var lockScope = GetAsyncLockable(cv).EnterScope();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
            Assert.IsFalse(waitFunc(IAsyncConditionVariable_NegativeTimeout));
    }

    [TestMethod]
    public async Task IAsyncConditionVariable_WaitAsync_DoesNotCompleteWithoutNotify()
    {
        var cv = CreateAsyncConditionVariable();
        using var lockScope = await GetAsyncLockable(cv).EnterScopeAsync();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
            Assert.IsFalse(await waitFunc(IAsyncConditionVariable_NegativeTimeout));
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [Timeout(IAsyncConditionVariable_TestTimeout)]
    public void IAsyncConditionVariable_Wait_CompletesAfterNotify()
    {
        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
            IAsyncConditionVariable_Wait_CompletesAfterNotify_Core(cv, waitFunc);
    }

    [TestMethod]
    [Timeout(IAsyncConditionVariable_TestTimeout)]
    public async Task IAsyncConditionVariable_WaitAsync_CompletesAfterNotify()
    {
        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
            await IAsyncConditionVariable_WaitAsync_CompletesAfterNotify_Core(cv, waitFunc);
    }

    void IAsyncConditionVariable_Wait_CompletesAfterNotify_Core(
        IAsyncConditionVariable cv,
        Func<TimeSpan, bool> waitFunc)
    {
        var lockable = GetAsyncLockable(cv);
        using var lockScope = lockable.EnterScope();

        var timeout = IAsyncConditionVariable_PositiveTimeout;

        #region Notify

        async Task NotifyTask()
        {
            using (await lockable.EnterScopeAsync())
                cv.Notify();
        }

        Task notificationTask;
        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyTask);

        Assert.IsTrue(waitFunc(timeout));
        TaskBridge.Execute(notificationTask);

        #endregion

        #region NotifyAll

        async Task NotifyAllTask()
        {
            using (await lockable.EnterScopeAsync())
                cv.NotifyAll();
        }

        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyAllTask);

        Assert.IsTrue(waitFunc(timeout));
        TaskBridge.Execute(notificationTask);

        #endregion
    }

    async Task IAsyncConditionVariable_WaitAsync_CompletesAfterNotify_Core(
        IAsyncConditionVariable cv,
        Func<TimeSpan, Task<bool>> waitFunc)
    {
        var lockable = GetAsyncLockable(cv);
        using var lockScope = await lockable.EnterScopeAsync();

        var timeout = IAsyncConditionVariable_PositiveTimeout;

        #region Notify

        async Task NotifyTask()
        {
            using (await lockable.EnterScopeAsync())
                cv.Notify();
        }

        Task notificationTask;
        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyTask);

        Assert.IsTrue(await waitFunc(timeout));
        await notificationTask;

        #endregion

        #region NotifyAll

        async Task NotifyAllTask()
        {
            using (await lockable.EnterScopeAsync())
                cv.NotifyAll();
        }

        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyAllTask);

        Assert.IsTrue(await waitFunc(timeout));
        await notificationTask;

        #endregion
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [Timeout(IAsyncConditionVariable_TestTimeout)]
    public void IAsyncConditionVariable_Wait_OneCompletesAfterNotify()
    {
        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
            IAsyncConditionVariable_Wait_OneCompletesAfterNotify_Core(cv, waitFunc);
    }

    [TestMethod]
    [Timeout(IAsyncConditionVariable_TestTimeout)]
    public async Task IAsyncConditionVariable_WaitAsync_OneCompletesAfterNotify()
    {
        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
            await IAsyncConditionVariable_WaitAsync_OneCompletesAfterNotify_Core(cv, waitFunc);
    }

    void IAsyncConditionVariable_Wait_OneCompletesAfterNotify_Core(
        IAsyncConditionVariable cv,
        Func<TimeSpan, bool> waitFunc)
    {
        var lockable = GetAsyncLockable(cv);
        var lockAcquiredEvent1 = new AsyncAutoResetEvent(false);
        var lockAcquiredEvent2 = new AsyncAutoResetEvent(false);

        bool WaitTask(TimeSpan timeout, AsyncAutoResetEvent lockAcquiredEvent)
        {
            using (lockable.EnterScope())
            {
                lockAcquiredEvent.Set();
                return waitFunc(timeout);
            }
        }

        Task<bool> waitTask1;
        using (ExecutionContext.SuppressFlow())
            waitTask1 = TaskBridge.ExecuteAsync(() => WaitTask(IAsyncConditionVariable_PositiveTimeout, lockAcquiredEvent1));

        Assert.IsTrue(lockAcquiredEvent1.Wait(IAsyncConditionVariable_PositiveTimeout));

        Task<bool> waitTask2;
        using (ExecutionContext.SuppressFlow())
            waitTask2 = TaskBridge.ExecuteAsync(() => WaitTask(IAsyncConditionVariable_NegativeTimeout, lockAcquiredEvent2));

        Assert.IsTrue(lockAcquiredEvent2.Wait(IAsyncConditionVariable_PositiveTimeout));

        async Task NotifyTask()
        {
            using (await lockable.EnterScopeAsync())
                cv.Notify();
        }

        Task notificationTask;
        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyTask);

        var waitResults = TaskBridge.Execute(Task.WhenAll(waitTask1, waitTask2));

        Assert.IsTrue(waitResults[0]);
        Assert.IsFalse(waitResults[1]);

        TaskBridge.Execute(notificationTask);
    }

    async Task IAsyncConditionVariable_WaitAsync_OneCompletesAfterNotify_Core(
        IAsyncConditionVariable cv,
        Func<TimeSpan, Task<bool>> waitFunc)
    {
        var lockable = GetAsyncLockable(cv);
        var lockAcquiredEvent1 = new AsyncAutoResetEvent(false);
        var lockAcquiredEvent2 = new AsyncAutoResetEvent(false);

        async Task<bool> WaitTask(TimeSpan timeout, AsyncAutoResetEvent lockAcquiredEvent)
        {
            using (await lockable.EnterScopeAsync())
            {
                lockAcquiredEvent.Set();
                return await waitFunc(timeout);
            }
        }

        Task<bool> waitTask1;
        using (ExecutionContext.SuppressFlow())
            waitTask1 = Task.Run(() => WaitTask(IAsyncConditionVariable_PositiveTimeout, lockAcquiredEvent1));

        Assert.IsTrue(await lockAcquiredEvent1.WaitAsync(IAsyncConditionVariable_PositiveTimeout));

        Task<bool> waitTask2;
        using (ExecutionContext.SuppressFlow())
            waitTask2 = Task.Run(() => WaitTask(IAsyncConditionVariable_NegativeTimeout, lockAcquiredEvent2));

        Assert.IsTrue(await lockAcquiredEvent2.WaitAsync(IAsyncConditionVariable_PositiveTimeout));

        async Task NotifyTask()
        {
            using (await lockable.EnterScopeAsync())
                cv.Notify();
        }

        Task notificationTask;
        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyTask);

        var waitResults = await Task.WhenAll(waitTask1, waitTask2);

        Assert.IsTrue(waitResults[0]);
        Assert.IsFalse(waitResults[1]);

        await notificationTask;
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void IAsyncConditionVariable_Wait_AllCompleteAfterNotifyAll()
    {
        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitFunctions(cv))
            IAsyncConditionVariable_Wait_AllCompleteAfterNotifyAll_Core(cv, waitFunc);
    }

    [TestMethod]
    public async Task IAsyncConditionVariable_WaitAsync_AllCompleteAfterNotifyAll()
    {
        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
            await IAsyncConditionVariable_WaitAsync_AllCompleteAfterNotifyAll_Core(cv, waitFunc);
    }

    void IAsyncConditionVariable_Wait_AllCompleteAfterNotifyAll_Core(
        IAsyncConditionVariable cv,
        Func<TimeSpan, bool> waitFunc)
    {
        var lockable = GetAsyncLockable(cv);
        var lockAcquiredEvent1 = new AsyncAutoResetEvent(false);
        var lockAcquiredEvent2 = new AsyncAutoResetEvent(false);

        bool WaitTask(TimeSpan timeout, AsyncAutoResetEvent lockAcquiredEvent)
        {
            using (lockable.EnterScope())
            {
                lockAcquiredEvent.Set();
                return waitFunc(timeout);
            }
        }

        Task<bool> waitTask1;
        using (ExecutionContext.SuppressFlow())
            waitTask1 = TaskBridge.ExecuteAsync(() => WaitTask(IAsyncConditionVariable_PositiveTimeout, lockAcquiredEvent1));

        Task<bool> waitTask2;
        using (ExecutionContext.SuppressFlow())
            waitTask2 = TaskBridge.ExecuteAsync(() => WaitTask(IAsyncConditionVariable_PositiveTimeout, lockAcquiredEvent2));

        Assert.IsTrue(lockAcquiredEvent1.Wait(IAsyncConditionVariable_PositiveTimeout));
        Assert.IsTrue(lockAcquiredEvent2.Wait(IAsyncConditionVariable_PositiveTimeout));

        async Task NotifyTask()
        {
            using (await lockable.EnterScopeAsync())
                cv.NotifyAll();
        }

        Task notificationTask;
        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyTask);

        var waitResults = TaskBridge.Execute(Task.WhenAll(waitTask1, waitTask2));

        Assert.IsTrue(waitResults[0]);
        Assert.IsTrue(waitResults[1]);

        TaskBridge.Execute(notificationTask);
    }

    async Task IAsyncConditionVariable_WaitAsync_AllCompleteAfterNotifyAll_Core(
        IAsyncConditionVariable cv,
        Func<TimeSpan, Task<bool>> waitFunc)
    {
        var lockable = GetAsyncLockable(cv);
        var lockAcquiredEvent1 = new AsyncAutoResetEvent(false);
        var lockAcquiredEvent2 = new AsyncAutoResetEvent(false);

        async Task<bool> WaitTask(TimeSpan timeout, AsyncAutoResetEvent lockAcquiredEvent)
        {
            using (await lockable.EnterScopeAsync())
            {
                lockAcquiredEvent.Set();
                return await waitFunc(timeout);
            }
        }

        Task<bool> waitTask1;
        using (ExecutionContext.SuppressFlow())
            waitTask1 = Task.Run(() => WaitTask(IAsyncConditionVariable_PositiveTimeout, lockAcquiredEvent1));

        Task<bool> waitTask2;
        using (ExecutionContext.SuppressFlow())
            waitTask2 = Task.Run(() => WaitTask(IAsyncConditionVariable_PositiveTimeout, lockAcquiredEvent2));

        Assert.IsTrue(await lockAcquiredEvent1.WaitAsync(IAsyncConditionVariable_PositiveTimeout));
        Assert.IsTrue(await lockAcquiredEvent2.WaitAsync(IAsyncConditionVariable_PositiveTimeout));

        async Task NotifyTask()
        {
            using (await lockable.EnterScopeAsync())
                cv.NotifyAll();
        }

        Task notificationTask;
        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyTask);

        var waitResults = await Task.WhenAll(waitTask1, waitTask2);

        Assert.IsTrue(waitResults[0]);
        Assert.IsTrue(waitResults[1]);

        await notificationTask;
    }

    // ----------------------------------------------------------------------

    #region Helpers

    static IEnumerable<Func<TimeSpan, bool>> EnumerateWaitFunctions(IAsyncConditionVariable cv)
    {
        yield return timeout => cv.Wait(TimeoutHelper.GetMillisecondsTimeout(timeout));
        yield return timeout => cv.Wait(TimeoutHelper.GetMillisecondsTimeout(timeout), CancellationToken.None);
        yield return timeout => cv.Wait(timeout);
        yield return timeout => cv.Wait(timeout, CancellationToken.None);

        yield return
            timeout =>
            {
                if (timeout >= IAsyncConditionVariable_PositiveTimeout)
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
                if (timeout >= IAsyncConditionVariable_PositiveTimeout)
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

    static IEnumerable<Func<TimeSpan, Task<bool>>> EnumerateWaitAsyncFunctions(IAsyncConditionVariable cv)
    {
        yield return timeout => cv.WaitAsync(TimeoutHelper.GetMillisecondsTimeout(timeout));
        yield return timeout => cv.WaitAsync(TimeoutHelper.GetMillisecondsTimeout(timeout), CancellationToken.None);
        yield return timeout => cv.WaitAsync(timeout);
        yield return timeout => cv.WaitAsync(timeout, CancellationToken.None);

        yield return
            async timeout =>
            {
                if (timeout >= IAsyncConditionVariable_PositiveTimeout)
                {
                    await cv.WaitAsync();
                    return true;
                }
                else
                {
                    return await cv.WaitAsync(timeout);
                }
            };

        yield return
            async timeout =>
            {
                if (timeout >= IAsyncConditionVariable_PositiveTimeout)
                {
                    await cv.WaitAsync(CancellationToken.None);
                    return true;
                }
                else
                {
                    return await cv.WaitAsync(timeout, CancellationToken.None);
                }
            };
    }

    #endregion
}
