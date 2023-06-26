using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public class AsyncRecursiveMutexTests
{
    [TestMethod]
    public async Task AsyncRecursiveMutex_LockAsync_Recursion_Nesting()
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
    public async Task AsyncRecursiveMutex_LockAsync_Recursion_Rollback()
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
    }
}
