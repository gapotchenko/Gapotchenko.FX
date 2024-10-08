﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("async")]
public abstract class IAsyncConditionVariableTests : IConditionVariableTests
{
    protected abstract IAsyncConditionVariable CreateAsyncConditionVariable();

    protected abstract IAsyncLockable GetAsyncLockable(IAsyncConditionVariable conditionVariable);

    protected sealed override IConditionVariable CreateConditionVariable() => CreateAsyncConditionVariable();

    protected sealed override ILockable GetLockable(IConditionVariable conditionVariable) => GetAsyncLockable((IAsyncConditionVariable)conditionVariable);

    // ----------------------------------------------------------------------

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
            Assert.IsTrue(LockableHelper.IsLockHeld(lockable));
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

    [TestMethod]
    public async Task IAsyncConditionVariable_WaitAsync_DoesNotCompleteWithoutNotify()
    {
        var cv = CreateAsyncConditionVariable();
        using var lockScope = await GetAsyncLockable(cv).EnterScopeAsync();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
            Assert.IsFalse(await waitFunc(IConditionVariable_NegativeTimeout));
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [Timeout(IConditionVariable_TestTimeout)]
    public async Task IAsyncConditionVariable_WaitAsync_CompletesAfterNotify()
    {
        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
            await IAsyncConditionVariable_WaitAsync_CompletesAfterNotify_Core(cv, waitFunc);
    }

    async Task IAsyncConditionVariable_WaitAsync_CompletesAfterNotify_Core(
        IAsyncConditionVariable cv,
        Func<TimeSpan, Task<bool>> waitFunc)
    {
        var lockable = GetAsyncLockable(cv);
        using var lockScope = await lockable.EnterScopeAsync();

        var timeout = IConditionVariable_PositiveTimeout;

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
    [Timeout(IConditionVariable_TestTimeout)]
    public async Task IAsyncConditionVariable_WaitAsync_OneCompletesAfterNotify()
    {
        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
            await IAsyncConditionVariable_WaitAsync_OneCompletesAfterNotify_Core(cv, waitFunc);
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
            waitTask1 = Task.Run(() => WaitTask(IConditionVariable_PositiveTimeout, lockAcquiredEvent1));

        Assert.IsTrue(await lockAcquiredEvent1.WaitAsync(IConditionVariable_PositiveTimeout));

        Task<bool> waitTask2;
        using (ExecutionContext.SuppressFlow())
            waitTask2 = Task.Run(() => WaitTask(IConditionVariable_NegativeTimeout, lockAcquiredEvent2));

        Assert.IsTrue(await lockAcquiredEvent2.WaitAsync(IConditionVariable_PositiveTimeout));

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
    public async Task IAsyncConditionVariable_WaitAsync_AllCompleteAfterNotifyAll()
    {
        var cv = CreateAsyncConditionVariable();

        foreach (var waitFunc in EnumerateWaitAsyncFunctions(cv))
            await IAsyncConditionVariable_WaitAsync_AllCompleteAfterNotifyAll_Core(cv, waitFunc);
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
            waitTask1 = Task.Run(() => WaitTask(IConditionVariable_PositiveTimeout, lockAcquiredEvent1));

        Task<bool> waitTask2;
        using (ExecutionContext.SuppressFlow())
            waitTask2 = Task.Run(() => WaitTask(IConditionVariable_PositiveTimeout, lockAcquiredEvent2));

        Assert.IsTrue(await lockAcquiredEvent1.WaitAsync(IConditionVariable_PositiveTimeout));
        Assert.IsTrue(await lockAcquiredEvent2.WaitAsync(IConditionVariable_PositiveTimeout));

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

    static IEnumerable<Func<TimeSpan, Task<bool>>> EnumerateWaitAsyncFunctions(IAsyncConditionVariable cv)
    {
        yield return timeout => cv.WaitAsync(TimeoutHelper.GetMillisecondsTimeout(timeout));
        yield return timeout => cv.WaitAsync(TimeoutHelper.GetMillisecondsTimeout(timeout), CancellationToken.None);
        yield return timeout => cv.WaitAsync(timeout);
        yield return timeout => cv.WaitAsync(timeout, CancellationToken.None);

        yield return
            async timeout =>
            {
                if (timeout >= IConditionVariable_PositiveTimeout)
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
                if (timeout >= IConditionVariable_PositiveTimeout)
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