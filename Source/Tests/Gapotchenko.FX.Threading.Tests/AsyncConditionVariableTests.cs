// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public sealed class AsyncConditionVariableTests : IAsyncConditionVariableTests
{
    protected override IAsyncConditionVariable CreateAsyncConditionVariable() =>
        new AsyncConditionVariable(new AsyncMutex());

    protected override IAsyncLockable GetAsyncLockable(IAsyncConditionVariable conditionVariable) =>
        ((AsyncConditionVariable)conditionVariable).Lockable;

    [TestMethod]
    public void AsyncConditionVariable_Constructor_ThrowsOnNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new AsyncConditionVariable(null!));
    }
}
