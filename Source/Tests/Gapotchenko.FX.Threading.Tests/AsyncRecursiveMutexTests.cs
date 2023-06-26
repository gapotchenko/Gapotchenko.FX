using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public class AsyncRecursiveMutexTests
{
    [TestMethod]
    public void AsyncRecursiveMutex_Construction()
    {
        var mutex = new AsyncRecursiveMutex();
        Assert.IsFalse(mutex.IsLocked);
    }

    [TestMethod]
    public async Task AsyncRecursiveMutex_LockAsync_Nesting()
    {
        var mutex = new AsyncRecursiveMutex();

        await mutex.LockAsync();
        Assert.IsTrue(mutex.IsLocked);

        Assert.IsTrue(mutex.TryLock());
        Assert.IsTrue(mutex.IsLocked);

        mutex.Unlock();
        Assert.IsTrue(mutex.IsLocked);

        mutex.Unlock();
        Assert.IsFalse(mutex.IsLocked);
    }

    [TestMethod]
    public async Task AsyncRecursiveMutex_LockAsync_Rollback()
    {
        var mutex = new AsyncRecursiveMutex();

        var cts = new CancellationTokenSource();
        cts.Cancel();

        bool wasCanceled = false;
        try
        {
            await mutex.LockAsync(cts.Token);
        }
        catch
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(mutex.IsLocked);

        Assert.IsTrue(mutex.TryLock());
        Assert.IsTrue(mutex.IsLocked);
    }

    [TestMethod]
    public async Task AsyncRecursiveMutex_TryLockAsync_TimeSpan_Nesting()
    {
        var mutex = new AsyncRecursiveMutex();
        var timeout = TimeSpan.FromMilliseconds(0);

        Assert.IsTrue(await mutex.TryLockAsync(timeout));
        Assert.IsTrue(mutex.IsLocked);

        Assert.IsTrue(await mutex.TryLockAsync(timeout));
        Assert.IsTrue(mutex.IsLocked);

        mutex.Unlock();
        Assert.IsTrue(mutex.IsLocked);

        mutex.Unlock();
        Assert.IsFalse(mutex.IsLocked);
    }

    [TestMethod]
    public async Task AsyncRecursiveMutex_TryLockAsync_TimeSpan_Rollback()
    {
        var mutex = new AsyncRecursiveMutex();
        var timeout = Timeout.InfiniteTimeSpan;

        var cts = new CancellationTokenSource();
        cts.Cancel();

        bool wasCanceled = false;
        try
        {
            await mutex.TryLockAsync(timeout, cts.Token);
        }
        catch
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(mutex.IsLocked);

        Assert.IsTrue(mutex.TryLock());
        Assert.IsTrue(mutex.IsLocked);
    }

    [TestMethod]
    public async Task AsyncRecursiveMutex_TryLockAsync_Int32_Nesting()
    {
        var mutex = new AsyncRecursiveMutex();
        var timeout = 0;

        Assert.IsTrue(await mutex.TryLockAsync(timeout));
        Assert.IsTrue(mutex.IsLocked);

        Assert.IsTrue(await mutex.TryLockAsync(timeout));
        Assert.IsTrue(mutex.IsLocked);

        mutex.Unlock();
        Assert.IsTrue(mutex.IsLocked);

        mutex.Unlock();
        Assert.IsFalse(mutex.IsLocked);
    }

    [TestMethod]
    public async Task AsyncRecursiveMutex_TryLockAsync_Int32_Rollback()
    {
        var mutex = new AsyncRecursiveMutex();
        var timeout = Timeout.Infinite;

        var cts = new CancellationTokenSource();
        cts.Cancel();

        bool wasCanceled = false;
        try
        {
            await mutex.TryLockAsync(timeout, cts.Token);
        }
        catch
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(mutex.IsLocked);

        Assert.IsTrue(mutex.TryLock());
        Assert.IsTrue(mutex.IsLocked);
    }
}
