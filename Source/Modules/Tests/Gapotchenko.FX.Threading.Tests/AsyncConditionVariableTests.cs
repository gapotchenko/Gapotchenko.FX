// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

public abstract class AsyncConditionVariableTests : IAsyncConditionVariableTests
{
    protected abstract IAsyncLockable CreateAsyncLockable();

    protected sealed override IAsyncConditionVariable CreateAsyncConditionVariable() =>
        new AsyncConditionVariable(CreateAsyncLockable());

    protected sealed override IAsyncLockable GetAsyncLockable(IAsyncConditionVariable conditionVariable) =>
        ((AsyncConditionVariable)conditionVariable).Lockable;

    [TestMethod]
    public void AsyncConditionVariable_Constructor_ThrowsOnNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new AsyncConditionVariable(null!));
    }
}

[TestClass]
public sealed class AsyncConditionVariableTests_NonRecursive : AsyncConditionVariableTests
{
    protected override IAsyncLockable CreateAsyncLockable() => new AsyncCriticalSection();
}

[TestClass]
[TestCategory("recursive")]
public sealed class AsyncConditionVariableTests_Recursive : AsyncConditionVariableTests
{
    protected override IAsyncLockable CreateAsyncLockable() => new AsyncLock();
}
