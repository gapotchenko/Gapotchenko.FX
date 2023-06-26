using Gapotchenko.FX.Linq.Operators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

readonly struct AsyncLockableTestsImpl
{
    public AsyncLockableTestsImpl(
        Func<IAsyncLockable> createLockableFunc,
        Func<IAsyncLockable, bool>? isLockedFunc)
    {
        m_CreateLockableFunc = createLockableFunc;
        m_IsLockedFunc = isLockedFunc;
    }

    readonly Func<IAsyncLockable> m_CreateLockableFunc;
    readonly Func<IAsyncLockable, bool>? m_IsLockedFunc;

    IAsyncLockable CreateLockable() => m_CreateLockableFunc();

    bool? IsLocked(IAsyncLockable lockable) => m_IsLockedFunc?.Invoke(lockable);

    void AssertIsLocked(IAsyncLockable lockable) => IsLocked(lockable)?.PipeOperator(Assert.IsTrue);

    void AssertIsNotLocked(IAsyncLockable lockable) => IsLocked(lockable)?.PipeOperator(Assert.IsFalse);

    // ----------------------------------------------------------------------

    public void Constuction()
    {
        var lockable = CreateLockable();
        AssertIsNotLocked(lockable);
    }

    // ----------------------------------------------------------------------

    public void Lock_Nesting()
    {
        var lockable = CreateLockable();

        lockable.Lock();
        AssertIsLocked(lockable);

        Assert.IsTrue(lockable.TryLock());
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsNotLocked(lockable);
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

        AssertIsNotLocked(lockable);

        Assert.IsTrue(lockable.TryLock());
        AssertIsLocked(lockable);
    }

    // ----------------------------------------------------------------------

    public async Task LockAsync_Nesting()
    {
        var lockable = CreateLockable();

        await lockable.LockAsync();
        AssertIsLocked(lockable);

        Assert.IsTrue(lockable.TryLock());
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsNotLocked(lockable);
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

        AssertIsNotLocked(lockable);

        Assert.IsTrue(lockable.TryLock());
        AssertIsLocked(lockable);
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
        Assert.IsTrue(tryLockFunc(lockable));
        AssertIsLocked(lockable);

        Assert.IsTrue(tryLockFunc(lockable));
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsNotLocked(lockable);
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

        AssertIsNotLocked(lockable);

        Assert.IsTrue(lockable.TryLock());
        AssertIsLocked(lockable);
    }

    // ----------------------------------------------------------------------

    async Task TryLockAsync_Nesting_Core(IAsyncLockable lockable, Func<Task<bool>> tryLockAsyncFunc)
    {
        Assert.IsTrue(await tryLockAsyncFunc());
        AssertIsLocked(lockable);

        Assert.IsTrue(await tryLockAsyncFunc());
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsNotLocked(lockable);
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public async Task TryLockAsync_TimeSpan_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = TimeSpan.Zero;

        await TryLockAsync_Nesting_Core(lockable, x => x.TryLockAsync(timeout));
        await TryLockAsync_Nesting_Core(lockable, x => x.TryLockAsync(timeout, CancellationToken.None));
    }

    [TestMethod]
    public async Task TryLockAsync_Int32_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = 0;

        await TryLockAsync_Nesting_Core(lockable, x => x.TryLockAsync(timeout));
        await TryLockAsync_Nesting_Core(lockable, x => x.TryLockAsync(timeout, CancellationToken.None));
    }

    async Task TryLockAsync_Nesting_Core(IAsyncLockable lockable, Func<IAsyncLockable, Task<bool>> tryLockAsyncFunc)
    {
        Assert.IsTrue(await tryLockAsyncFunc(lockable));
        AssertIsLocked(lockable);

        Assert.IsTrue(await tryLockAsyncFunc(lockable));
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsLocked(lockable);

        lockable.Unlock();
        AssertIsNotLocked(lockable);
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

        AssertIsNotLocked(lockable);

        Assert.IsTrue(lockable.TryLock());
        AssertIsLocked(lockable);
    }

    // ----------------------------------------------------------------------

    public void Unlock_NonLocked()
    {
        var lockable = CreateLockable();
        Assert.ThrowsException<InvalidOperationException>(lockable.Unlock);
    }
}
