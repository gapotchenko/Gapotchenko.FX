﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
[TestCategory("recursive")]
public sealed class AsyncMonitorTests : IAsyncMonitorTests
{
    protected override IAsyncMonitor CreateAsyncMonitor() => new AsyncMonitor();

    protected override IAsyncMonitor GetAsyncMonitorFor(object obj) => AsyncMonitor.For(obj);
}

[TestClass]
[TestCategory("monitor")]
[TestCategory("recursive")]
public sealed class AsyncRecursiveMonitorTests_IAsyncLockable : IAsyncLockableTests
{
    protected override IAsyncLockable CreateAsyncLockable() => new AsyncMonitor();
}