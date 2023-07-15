using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("async")]
[TestCategory("lockable")]
public abstract class AsyncLockableTestsBase
{
    protected AsyncLockableTestsBase()
    {
        m_Impl = new(CreateAsyncLockable);
    }

    readonly AsyncLockableTestsImpl m_Impl;

    protected abstract IAsyncLockable CreateAsyncLockable();

    [TestMethod]
    public void AsyncLockable_Construction() => m_Impl.Constuction();

    [TestMethod]
    public void AsyncLockable_Lock_Nesting() => m_Impl.Lock_Nesting();

    [TestMethod]
    public void AsyncLockable_Lock_Rollback() => m_Impl.Lock_Rollback();

    [TestMethod]
    public Task AsyncLockable_LockAsync_Nesting() => m_Impl.LockAsync_Nesting();

    [TestMethod]
    public Task AsyncLockable_LockAsync_Rollback() => m_Impl.LockAsync_Rollback();

    [TestMethod]
    public void AsyncLockable_TryLock_Nesting() => m_Impl.TryLock_Nesting();

    [TestMethod]
    public void AsyncLockable_TryLock_TimeSpan_Nesting() => m_Impl.TryLock_TimeSpan_Nesting();

    [TestMethod]
    public void AsyncLockable_TryLock_Int32_Nesting() => m_Impl.TryLock_Int32_Nesting();

    [TestMethod]
    public void AsyncLockable_TryLock_TimeSpan_Rollback() => m_Impl.TryLock_TimeSpan_Rollback();

    [TestMethod]
    public void AsyncLockable_TryLock_Int32_Rollback() => m_Impl.TryLock_Int32_Rollback();

    [TestMethod]
    public Task AsyncLockable_TryLockAsync_TimeSpan_Nesting() => m_Impl.TryLockAsync_TimeSpan_Nesting();

    [TestMethod]
    public Task AsyncLockable_TryLockAsync_Int32_Nesting() => m_Impl.TryLockAsync_Int32_Nesting();

    [TestMethod]
    public Task AsyncLockable_TryLockAsync_TimeSpan_Rollback() => m_Impl.TryLockAsync_TimeSpan_Rollback();

    [TestMethod]
    public Task AsyncLockable_TryLockAsync_Int32_Rollback() => m_Impl.TryLockAsync_Int32_Rollback();

    [TestMethod]
    public void AsyncLockable_Unlock_NonLocked() => m_Impl.Unlock_NonLocked();
}
