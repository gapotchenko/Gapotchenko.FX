// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

#if PREVIEW

[TestClass]
public sealed class AsyncMonitorSlimTests : IAsyncMonitorTests
{
    protected override IAsyncMonitor CreateAsyncMonitor() => new AsyncMonitorSlim();

    protected override IAsyncMonitor GetAsyncMonitorFor(object obj) => AsyncMonitorSlim.For(obj);
}

[TestClass]
[TestCategory("monitor")]
public sealed class AsyncMonitorTests_IAsyncLockable : IAsyncLockableTests
{
    protected override IAsyncLockable CreateAsyncLockable() => new AsyncMonitorSlim();
}

#endif
