// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
[TestCategory("recursive")]
public sealed class AsyncMonitorTests : IAsyncMonitorTests
{
    protected override IAsyncMonitor CreateAsyncMonitor() => new AsyncMonitor();

    protected override IAsyncMonitor GetAsyncMonitorFor(object obj) => AsyncMonitor.For(obj);

    protected override bool IsRecursive => true;
}

[TestClass]
[TestCategory("monitor")]
[TestCategory("recursive")]
public sealed class AsyncMonitorTests_IAsyncLockable : IAsyncLockableTests
{
    protected override IAsyncLockable CreateAsyncLockable() => new AsyncMonitor();
}
