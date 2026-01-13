// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public sealed class AsyncCrtiticalSectionTests : IAsyncMutexTests
{
    protected override IAsyncMutex CreateAsyncMutex() => new AsyncCriticalSection();
}
