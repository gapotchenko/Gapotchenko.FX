using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("lockable")]
public abstract class ILockableTests
{
    protected abstract ILockable CreateLockable();

    // ----------------------------------------------------------------------

    [TestMethod]
    public void ILockable_Construction()
    {
        var lockable = CreateLockable();
        Assert.AreEqual(lockable is IRecursiveLockable, lockable.IsRecursive);
        Assert.IsFalse(lockable.IsEntered);
        if (lockable is IRecursiveLockable recursiveLockable)
            Assert.IsFalse(recursiveLockable.IsLockedByCurrentThread);
    }

    [TestMethod]
    public void ILockable_Enter_Nesting()
    {
        var lockable = CreateLockable();

        if (lockable is IAsyncRecursiveLockable recursiveLockable)
        {
            recursiveLockable.Enter();
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            Assert.IsTrue(recursiveLockable.TryEnter());
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            recursiveLockable.Exit();
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            recursiveLockable.Exit();
            Assert.IsFalse(recursiveLockable.IsEntered);
            Assert.IsFalse(recursiveLockable.IsLockedByCurrentThread);
        }
        else
        {
            lockable.Enter();
            Assert.IsTrue(lockable.IsEntered);

            Assert.IsFalse(lockable.TryEnter());
            Assert.IsTrue(lockable.IsEntered);

            lockable.Exit();
            Assert.IsFalse(lockable.IsEntered);
        }
    }

    [TestMethod]
    public void ILockable_Enter_Rollback()
    {
        var lockable = CreateLockable();

        bool wasCanceled = false;
        try
        {
            lockable.Enter(new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsEntered);

        Assert.IsTrue(lockable.TryEnter());
        Assert.IsTrue(lockable.IsEntered);
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void ILockable_TryEnter_Nesting() =>
        TryEnter_Nesting_Core(CreateLockable(), x => x.TryEnter());

    [TestMethod]
    public void ILockable_TryEnter_TimeSpan_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = TimeSpan.Zero;

        TryEnter_Nesting_Core(lockable, x => x.TryEnter(timeout));
        TryEnter_Nesting_Core(lockable, x => x.TryEnter(timeout, CancellationToken.None));
    }

    [TestMethod]
    public void ILockable_TryEnter_Int32_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = 0;

        TryEnter_Nesting_Core(lockable, x => x.TryEnter(timeout));
        TryEnter_Nesting_Core(lockable, x => x.TryEnter(timeout, CancellationToken.None));
    }

    static void TryEnter_Nesting_Core(ILockable lockable, Func<ILockable, bool> tryLockFunc)
    {
        if (lockable is IRecursiveLockable recursiveLockable)
        {
            Assert.IsTrue(tryLockFunc(recursiveLockable));
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            Assert.IsTrue(tryLockFunc(recursiveLockable));
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            recursiveLockable.Exit();
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            recursiveLockable.Exit();
            Assert.IsFalse(recursiveLockable.IsEntered);
            Assert.IsFalse(recursiveLockable.IsLockedByCurrentThread);
        }
        else
        {
            Assert.IsTrue(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsEntered);

            Assert.IsFalse(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsEntered);

            lockable.Exit();
            Assert.IsFalse(lockable.IsEntered);
        }
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void ILockable_TryEnter_TimeSpan_Rollback() =>
        TryEnter_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryEnter(Timeout.InfiniteTimeSpan, ct));

    [TestMethod]
    public void ILockable_TryEnter_Int32_Rollback() =>
        TryEnter_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryEnter(Timeout.Infinite, ct));

    static void TryEnter_Rollback_Core(ILockable lockable, Func<ILockable, CancellationToken, bool> tryLockFunc)
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

        Assert.IsFalse(lockable.IsEntered);

        Assert.IsTrue(lockable.TryEnter());
        Assert.IsTrue(lockable.IsEntered);
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public void ILockable_Exit_NonLocked()
    {
        var lockable = CreateLockable();
        Assert.ThrowsExactly<SynchronizationLockException>(lockable.Exit);
    }
}
