// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
[TestCategory("manual")]
public sealed class AsyncManualResetEventTests : IAsyncResetEventTests
{
    protected override bool IsAutoReset => false;

    protected override IAsyncResetEvent CreateAsyncResetEvent() => new AsyncManualResetEvent();

    protected override IAsyncResetEvent CreateAsyncResetEvent(bool initialState) => new AsyncManualResetEvent(initialState);
}
