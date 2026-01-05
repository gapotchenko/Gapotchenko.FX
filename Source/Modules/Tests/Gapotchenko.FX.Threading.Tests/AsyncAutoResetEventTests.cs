// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
[TestCategory("auto")]
public sealed class AsyncAutoResetEventTests : IAsyncResetEventTests
{
    protected override bool IsAutoReset => true;

    protected override IAsyncResetEvent CreateAsyncResetEvent(bool initialState) => new AsyncAutoResetEvent(initialState);
}
