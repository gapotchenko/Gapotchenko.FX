// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public sealed class AsyncRecursiveMonitorTests : IAsyncMonitorTests
{
    protected override IAsyncMonitor CreateAsyncMonitor() => new AsyncRecursiveMonitor();

    protected override IAsyncMonitor GetAsyncMonitorFor(object obj) => AsyncRecursiveMonitor.For(obj);
}

[TestClass]
[TestCategory("monitor")]
public sealed class AsyncRecursiveMonitorTests_IAsyncLockable : IAsyncLockableTests
{
    protected override IAsyncLockable CreateAsyncLockable() => new AsyncRecursiveMonitor();
}
