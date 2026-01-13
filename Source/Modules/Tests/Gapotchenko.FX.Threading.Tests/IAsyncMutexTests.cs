// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("mutex")]
public abstract class IAsyncMutexTests : IAsyncLockableTests
{
    protected abstract IAsyncMutex CreateAsyncMutex();

    protected sealed override IAsyncLockable CreateAsyncLockable() => CreateAsyncMutex();

    [TestMethod]
    public void IAsyncMutex_Constructor()
    {
        var mutex = CreateAsyncMutex();
        Assert.AreEqual(mutex.IsRecursive, mutex is IAsyncRecursiveMutex);
    }
}
