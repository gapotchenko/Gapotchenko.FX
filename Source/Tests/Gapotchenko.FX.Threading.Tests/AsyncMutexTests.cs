﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
[TestCategory("mutex")]
public sealed class AsyncMutexTests : IAsyncLockableTests
{
    protected override IAsyncLockable CreateAsyncLockable() => new AsyncMutex();
}
