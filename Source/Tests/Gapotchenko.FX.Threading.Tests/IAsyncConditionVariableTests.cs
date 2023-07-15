// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("async")]
[TestCategory("condition-variable")]
public abstract class IAsyncConditionVariableTests
{
    protected static readonly TimeSpan IAsyncConditionVariable_PositiveTimeout = TimeSpan.FromMilliseconds(30000);
    protected static readonly TimeSpan IAsyncConditionVariable_NegativeTimeout = TimeSpan.FromMilliseconds(200);

    // ----------------------------------------------------------------------

    protected abstract IAsyncConditionVariable CreateAsyncConditionVariable();

    protected abstract IAsyncLockable GetAsyncLockable(IAsyncConditionVariable conditionVariable);

    // ----------------------------------------------------------------------

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    [DataRow(Timeout.Infinite)]
    public void IAsyncConditionVariable_Wait_ThrowsOnUnlockedLockable(int millisecondsTimeout)
    {
        var cv = CreateAsyncConditionVariable();

        Assert.ThrowsException<SynchronizationLockException>(cv.Wait);
        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(CancellationToken.None));

        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(millisecondsTimeout));
        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(millisecondsTimeout, CancellationToken.None));

        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(timeout));
        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(timeout, CancellationToken.None));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    [DataRow(Timeout.Infinite)]
    public async Task IAsyncConditionVariable_WaitAsync_ThrowsOnUnlockedLockable(int millisecondsTimeout)
    {
        var cv = CreateAsyncConditionVariable();

        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(cv.WaitAsync);
        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(CancellationToken.None));

        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(millisecondsTimeout));
        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(millisecondsTimeout, CancellationToken.None));

        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(timeout));
        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(timeout, CancellationToken.None));
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void IAsyncConditionVariable_Wait_LocksOnExit(int millisecondsTimeout)
    {
        var cv = CreateAsyncConditionVariable();
        var lockable = GetAsyncLockable(cv);
        using var lockScope = lockable.LockScope();

        Assert.IsFalse(cv.Wait(millisecondsTimeout));
        Assert.IsTrue(IsLockHeld(lockable));

        Assert.IsFalse(cv.Wait(millisecondsTimeout, CancellationToken.None));
        Assert.IsTrue(IsLockHeld(lockable));

        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        Assert.IsFalse(cv.Wait(timeout));
        Assert.IsTrue(IsLockHeld(lockable));

        Assert.IsFalse(cv.Wait(timeout, CancellationToken.None));
        Assert.IsTrue(IsLockHeld(lockable));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public async Task IAsyncConditionVariable_WaitAsync_LocksOnExit(int millisecondsTimeout)
    {
        var cv = CreateAsyncConditionVariable();
        var lockable = GetAsyncLockable(cv);
        using var lockScope = await lockable.LockScopeAsync();

        Assert.IsFalse(await cv.WaitAsync(millisecondsTimeout));
        Assert.IsTrue(IsLockHeld(lockable));

        Assert.IsFalse(await cv.WaitAsync(millisecondsTimeout, CancellationToken.None));
        Assert.IsTrue(IsLockHeld(lockable));

        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        Assert.IsFalse(await cv.WaitAsync(timeout));
        Assert.IsTrue(IsLockHeld(lockable));

        Assert.IsFalse(await cv.WaitAsync(timeout, CancellationToken.None));
        Assert.IsTrue(IsLockHeld(lockable));
    }

    static bool IsLockHeld(IAsyncLockable lockable)
    {
        if (lockable is IAsyncRecursiveLockable recursiveLockable)
            return recursiveLockable.IsLockHeld;
        else
            return lockable.IsLocked;
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void IAsyncConditionVariable_Wait_NoDeferredNotification(int millisecondsTimeout)
    {
        var cv = CreateAsyncConditionVariable();
        using var lockScope = GetAsyncLockable(cv).LockScope();

        cv.NotifyAll();
        Assert.IsFalse(cv.Wait(millisecondsTimeout));

        cv.NotifyAll();
        Assert.IsFalse(cv.Wait(millisecondsTimeout, CancellationToken.None));

        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        cv.NotifyAll();
        Assert.IsFalse(cv.Wait(timeout));

        cv.NotifyAll();
        Assert.IsFalse(cv.Wait(timeout, CancellationToken.None));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public async Task IAsyncConditionVariable_WaitAsync_NoDeferredNotification(int millisecondsTimeout)
    {
        var cv = CreateAsyncConditionVariable();
        using var lockScope = await GetAsyncLockable(cv).LockScopeAsync();

        cv.NotifyAll();
        Assert.IsFalse(await cv.WaitAsync(millisecondsTimeout));

        cv.NotifyAll();
        Assert.IsFalse(await cv.WaitAsync(millisecondsTimeout, CancellationToken.None));

        var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

        cv.NotifyAll();
        Assert.IsFalse(await cv.WaitAsync(timeout));

        cv.NotifyAll();
        Assert.IsFalse(await cv.WaitAsync(timeout, CancellationToken.None));
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void IAsyncConditionVariable_Wait_DoesNotCompleteWithoutNotify()
    {
        var cv = CreateAsyncConditionVariable();
        using var lockScope = GetAsyncLockable(cv).LockScope();

        var timeout = IAsyncConditionVariable_NegativeTimeout;
        var millisecondsTimeout = GetMillisecondsTimeout(timeout);

        Assert.IsFalse(cv.Wait(millisecondsTimeout));
        Assert.IsFalse(cv.Wait(millisecondsTimeout, CancellationToken.None));
        Assert.IsFalse(cv.Wait(timeout));
        Assert.IsFalse(cv.Wait(timeout, CancellationToken.None));
    }

    [TestMethod]
    public async Task IAsyncConditionVariable_WaitAsync_DoesNotCompleteWithoutNotify()
    {
        var cv = CreateAsyncConditionVariable();
        using var lockScope = await GetAsyncLockable(cv).LockScopeAsync();

        var timeout = IAsyncConditionVariable_NegativeTimeout;
        var millisecondsTimeout = GetMillisecondsTimeout(timeout);

        Assert.IsFalse(await cv.WaitAsync(millisecondsTimeout));
        Assert.IsFalse(await cv.WaitAsync(millisecondsTimeout, CancellationToken.None));
        Assert.IsFalse(await cv.WaitAsync(timeout));
        Assert.IsFalse(await cv.WaitAsync(timeout, CancellationToken.None));
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void IAsyncConditionVariable_Wait_CompletesAfterNotify()
    {
        var cv = CreateAsyncConditionVariable();

        IAsyncConditionVariable_Wait_CompletesAfterNotify_Core(
            cv,
            timeout => cv.Wait(GetMillisecondsTimeout(timeout)));

        IAsyncConditionVariable_Wait_CompletesAfterNotify_Core(
            cv,
            timeout => cv.Wait(GetMillisecondsTimeout(timeout), CancellationToken.None));

        IAsyncConditionVariable_Wait_CompletesAfterNotify_Core(
            cv,
            timeout => cv.Wait(timeout));

        IAsyncConditionVariable_Wait_CompletesAfterNotify_Core(
            cv,
            timeout => cv.Wait(timeout, CancellationToken.None));
    }

    [TestMethod]
    public async Task IAsyncConditionVariable_WaitAsync_CompletesAfterNotify()
    {
        var cv = CreateAsyncConditionVariable();

        await IAsyncConditionVariable_WaitAsync_CompletesAfterNotify_Core(
            cv,
            timeout => cv.WaitAsync(GetMillisecondsTimeout(timeout)));

        await IAsyncConditionVariable_WaitAsync_CompletesAfterNotify_Core(
            cv,
            timeout => cv.WaitAsync(GetMillisecondsTimeout(timeout), CancellationToken.None));

        await IAsyncConditionVariable_WaitAsync_CompletesAfterNotify_Core(
            cv,
            timeout => cv.WaitAsync(timeout));

        await IAsyncConditionVariable_WaitAsync_CompletesAfterNotify_Core(
            cv,
            timeout => cv.WaitAsync(timeout, CancellationToken.None));
    }

    void IAsyncConditionVariable_Wait_CompletesAfterNotify_Core(
        IAsyncConditionVariable cv,
        Func<TimeSpan, bool> waitFunc)
    {
        var lockable = GetAsyncLockable(cv);
        using var lockScope = lockable.LockScope();

        var timeout = IAsyncConditionVariable_PositiveTimeout;

        // -----------------------------------------

        async Task Notify()
        {
            using (await lockable.LockScopeAsync())
                cv.Notify();
        }

        Task notificationTask;
        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(Notify);

        Assert.IsTrue(waitFunc(timeout));
        TaskBridge.Execute(notificationTask);

        // -----------------------------------------

        async Task NotifyAll()
        {
            using (await lockable.LockScopeAsync())
                cv.NotifyAll();
        }

        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyAll);

        Assert.IsTrue(waitFunc(timeout));
        TaskBridge.Execute(notificationTask);
    }

    async Task IAsyncConditionVariable_WaitAsync_CompletesAfterNotify_Core(
        IAsyncConditionVariable cv,
        Func<TimeSpan, Task<bool>> waitFunc)
    {
        var lockable = GetAsyncLockable(cv);
        using var lockScope = await lockable.LockScopeAsync();

        var timeout = IAsyncConditionVariable_PositiveTimeout;

        // -----------------------------------------

        async Task Notify()
        {
            using (await lockable.LockScopeAsync())
                cv.Notify();
        }

        Task notificationTask;
        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(Notify);

        Assert.IsTrue(await waitFunc(timeout));
        await notificationTask;

        // -----------------------------------------

        async Task NotifyAll()
        {
            using (await lockable.LockScopeAsync())
                cv.NotifyAll();
        }

        using (ExecutionContext.SuppressFlow())
            notificationTask = Task.Run(NotifyAll);

        Assert.IsTrue(await waitFunc(timeout));
        await notificationTask;
    }

    static int GetMillisecondsTimeout(TimeSpan timeout) => checked((int)timeout.TotalMilliseconds);
}
