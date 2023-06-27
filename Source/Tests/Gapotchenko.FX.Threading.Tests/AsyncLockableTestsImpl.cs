using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Threading.Tests;

readonly struct AsyncLockableTestsImpl
{
    public AsyncLockableTestsImpl(Func<IAsyncLockable> createLockableFunc)
    {
        m_CreateLockableFunc = createLockableFunc;
    }

    readonly Func<IAsyncLockable> m_CreateLockableFunc;

    IAsyncLockable CreateLockable() => m_CreateLockableFunc();

    // ----------------------------------------------------------------------

    public void Constuction()
    {
        var lockable = CreateLockable();
        Assert.IsFalse(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

    public void Lock_Nesting()
    {
        var lockable = CreateLockable();

        if (lockable.IsRecursive)
        {
            lockable.Lock();
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsTrue(lockable.TryLock());
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }
        else
        {
            lockable.Lock();
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsFalse(lockable.TryLock());
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }
    }

    // ----------------------------------------------------------------------

    public void Lock_Rollback()
    {
        var lockable = CreateLockable();

        bool wasCanceled = false;
        try
        {
            lockable.Lock(new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsLocked);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

    static async Task VerifyLockingSemanticsAsync(
        IAsyncLockable lockable,
        Func<IAsyncLockable, CancellationToken, Task> lockAsyncFunc)
    {
        var lockEvent = new ManualResetEventSlim();
        var unlockEvent = new ManualResetEventSlim();
        var lockerTask = TaskBridge.ExecuteAsync(
            () =>
            {
                lockable.Lock();
                lockEvent.Set();
                unlockEvent.Wait();
                lockable.Unlock();
            });

        try
        {
            var cts = new CancellationTokenSource();
            lockEvent.Wait();
            cts.CancelAfter(1);

            bool wasCanceled = false;
            try
            {
                await lockAsyncFunc(lockable, cts.Token);
            }
            catch (OperationCanceledException)
            {
                wasCanceled = true;
            }
            Assert.IsTrue(wasCanceled);

            Assert.IsTrue(lockable.IsLocked);
            Assert.IsFalse(lockable.TryLock());
        }
        finally
        {
            unlockEvent.Set();
            await lockerTask;
        }

        Assert.IsFalse(lockable.IsLocked);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);

        lockable.Unlock();
        Assert.IsFalse(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

    public async Task LockAsync_Nesting()
    {
        var lockable = CreateLockable();

        if (lockable.IsRecursive)
        {
            await lockable.LockAsync();
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsTrue(lockable.TryLock());
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }
        else
        {
            await lockable.LockAsync();
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsFalse(lockable.TryLock());
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }

        // --------------------------------------------

        await VerifyLockingSemanticsAsync(
            lockable,
            (x, ct) => x.LockAsync(ct));
    }

    // ----------------------------------------------------------------------

    public async Task LockAsync_Rollback()
    {
        var lockable = CreateLockable();

        bool wasCanceled = false;
        try
        {
            await lockable.LockAsync(new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsLocked);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

    public void TryLock_Nesting() =>
        TryLock_Nesting_Core(CreateLockable(), x => x.TryLock());

    public void TryLock_TimeSpan_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = TimeSpan.Zero;

        TryLock_Nesting_Core(lockable, x => x.TryLock(timeout));
        TryLock_Nesting_Core(lockable, x => x.TryLock(timeout, CancellationToken.None));
    }

    public void TryLock_Int32_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = 0;

        TryLock_Nesting_Core(lockable, x => x.TryLock(timeout));
        TryLock_Nesting_Core(lockable, x => x.TryLock(timeout, CancellationToken.None));
    }

    void TryLock_Nesting_Core(IAsyncLockable lockable, Func<IAsyncLockable, bool> tryLockFunc)
    {
        if (lockable.IsRecursive)
        {
            Assert.IsTrue(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsTrue(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }
        else
        {
            Assert.IsTrue(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsFalse(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }
    }

    // ----------------------------------------------------------------------

    public void TryLock_TimeSpan_Rollback() =>
        TryLock_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryLock(Timeout.InfiniteTimeSpan, ct));

    public void TryLock_Int32_Rollback() =>
        TryLock_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryLock(Timeout.Infinite, ct));

    void TryLock_Rollback_Core(IAsyncLockable lockable, Func<IAsyncLockable, CancellationToken, bool> tryLockFunc)
    {
        bool wasCanceled = false;
        try
        {
            tryLockFunc(lockable, new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsLocked);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public async Task TryLockAsync_TimeSpan_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = TimeSpan.Zero;

        static Task LockAsync(IAsyncLockable lockable, CancellationToken cancellationToken) =>
            lockable.TryLockAsync(Timeout.InfiniteTimeSpan, cancellationToken);

        await TryLockAsync_Nesting_Core(
            lockable,
            x => x.TryLockAsync(timeout),
            LockAsync);

        await TryLockAsync_Nesting_Core(
            lockable,
            x => x.TryLockAsync(timeout, CancellationToken.None),
            LockAsync);
    }

    [TestMethod]
    public async Task TryLockAsync_Int32_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = 0;

        static Task LockAsync(IAsyncLockable lockable, CancellationToken cancellationToken) =>
            lockable.TryLockAsync(Timeout.Infinite, cancellationToken);

        await TryLockAsync_Nesting_Core(
            lockable,
            x => x.TryLockAsync(timeout),
            LockAsync);

        await TryLockAsync_Nesting_Core(
            lockable,
            x => x.TryLockAsync(timeout, CancellationToken.None),
            LockAsync);
    }

    async Task TryLockAsync_Nesting_Core(
        IAsyncLockable lockable,
        Func<IAsyncLockable, Task<bool>> tryLockAsyncFunc,
        Func<IAsyncLockable, CancellationToken, Task> lockAsyncFunc)
    {
        if (lockable.IsRecursive)
        {
            Assert.IsTrue(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsTrue(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }
        else
        {
            Assert.IsTrue(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsFalse(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }

        // --------------------------------------------

        await VerifyLockingSemanticsAsync(lockable, lockAsyncFunc);
    }

    // ----------------------------------------------------------------------

    public Task TryLockAsync_TimeSpan_Rollback() =>
        TryLockAsync_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryLockAsync(Timeout.InfiniteTimeSpan, ct));

    public Task TryLockAsync_Int32_Rollback() =>
        TryLockAsync_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryLockAsync(Timeout.Infinite, ct));

    async Task TryLockAsync_Rollback_Core(IAsyncLockable lockable, Func<IAsyncLockable, CancellationToken, Task<bool>> tryLockAsyncFunc)
    {
        bool wasCanceled = false;
        try
        {
            await tryLockAsyncFunc(lockable, new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsLocked);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

    public void Unlock_NonLocked()
    {
        var lockable = CreateLockable();
        Assert.ThrowsException<InvalidOperationException>(lockable.Unlock);
    }
}
