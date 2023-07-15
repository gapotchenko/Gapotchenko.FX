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
