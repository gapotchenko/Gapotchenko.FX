// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
[TestCategory("recursive")]
public sealed class AsyncRecursiveMutexTests : IAsyncMutexTests
{
    protected override IAsyncMutex CreateAsyncMutex() => new AsyncLock();
}
