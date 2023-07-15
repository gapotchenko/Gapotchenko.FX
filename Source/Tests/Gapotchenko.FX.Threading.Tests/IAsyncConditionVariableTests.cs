// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("async")]
[TestCategory("condition-variable")]
public abstract class IAsyncConditionVariableTests
{
    protected abstract IAsyncConditionVariable CreateAsyncConditionVariable();

    protected abstract IAsyncLockable GetAsyncLockable(IAsyncConditionVariable conditionVariable);

    [TestMethod]
    public void IAsyncConditionVariable_Wait_ThrowsOnUnlockedLockable()
    {
        var cv = CreateAsyncConditionVariable();

        Assert.ThrowsException<SynchronizationLockException>(cv.Wait);
        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(CancellationToken.None));
        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(0));
        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(0, CancellationToken.None));
        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(TimeSpan.Zero));
        Assert.ThrowsException<SynchronizationLockException>(() => cv.Wait(TimeSpan.Zero, CancellationToken.None));
    }

    [TestMethod]
    public async Task IAsyncConditionVariable_WaitAsync_ThrowsOnUnlockedLockable()
    {
        var cv = CreateAsyncConditionVariable();

        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(cv.WaitAsync);
        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(CancellationToken.None));
        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(0));
        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(0, CancellationToken.None));
        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(TimeSpan.Zero));
        await Assert.ThrowsExceptionAsync<SynchronizationLockException>(() => cv.WaitAsync(TimeSpan.Zero, CancellationToken.None));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void IAsyncConditionVariable_Wait_NoDeferredNotification(int millisecondsTimeout)
    {
        var cv = CreateAsyncConditionVariable();
        using var lockable = GetAsyncLockable(cv).LockScope();

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
        using var lockable = GetAsyncLockable(cv).LockScope();

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
}
