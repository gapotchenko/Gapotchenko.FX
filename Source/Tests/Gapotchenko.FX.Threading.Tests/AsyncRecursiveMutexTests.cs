// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public sealed class AsyncRecursiveMutexTests
{
    static readonly AsyncLockableTestsImpl m_LockableTestsImpl = new(() => new AsyncRecursiveMutex());

    [TestMethod]
    public void AsyncRecursiveMutex_Construction() => m_LockableTestsImpl.Constuction();

    [TestMethod]
    public void AsyncRecursiveMutex_Lock_Nesting() => m_LockableTestsImpl.Lock_Nesting();

    [TestMethod]
    public void AsyncRecursiveMutex_Lock_Rollback() => m_LockableTestsImpl.Lock_Rollback();

    [TestMethod]
    public Task AsyncRecursiveMutex_LockAsync_Nesting() => m_LockableTestsImpl.LockAsync_Nesting();

    [TestMethod]
    public Task AsyncRecursiveMutex_LockAsync_Rollback() => m_LockableTestsImpl.LockAsync_Rollback();

    [TestMethod]
    public void AsyncRecursiveMutex_TryLock_Nesting() => m_LockableTestsImpl.TryLock_Nesting();

    [TestMethod]
    public void AsyncRecursiveMutex_TryLock_TimeSpan_Nesting() => m_LockableTestsImpl.TryLock_TimeSpan_Nesting();

    [TestMethod]
    public void AsyncRecursiveMutex_TryLock_Int32_Nesting() => m_LockableTestsImpl.TryLock_Int32_Nesting();

    [TestMethod]
    public void AsyncRecursiveMutex_TryLock_TimeSpan_Rollback() => m_LockableTestsImpl.TryLock_TimeSpan_Rollback();

    [TestMethod]
    public void AsyncRecursiveMutex_TryLock_Int32_Rollback() => m_LockableTestsImpl.TryLock_Int32_Rollback();

    [TestMethod]
    public Task AsyncRecursiveMutex_TryLockAsync_TimeSpan_Nesting() => m_LockableTestsImpl.TryLockAsync_TimeSpan_Nesting();

    [TestMethod]
    public Task AsyncRecursiveMutex_TryLockAsync_Int32_Nesting() => m_LockableTestsImpl.TryLockAsync_Int32_Nesting();

    [TestMethod]
    public Task AsyncRecursiveMutex_TryLockAsync_TimeSpan_Rollback() => m_LockableTestsImpl.TryLockAsync_TimeSpan_Rollback();

    [TestMethod]
    public Task AsyncRecursiveMutex_TryLockAsync_Int32_Rollback() => m_LockableTestsImpl.TryLockAsync_Int32_Rollback();

    [TestMethod]
    public void AsyncRecursiveMutex_Unlock_NonLocked() => m_LockableTestsImpl.Unlock_NonLocked();
}
