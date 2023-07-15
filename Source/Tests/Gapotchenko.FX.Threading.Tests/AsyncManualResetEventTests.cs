// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
public sealed class AsyncManualResetEventTests : AsyncResetEventTestsBase
{
    protected override bool IsAutoReset => false;

    protected override IAsyncResetEvent CreateAsyncResetEvent() => new AsyncManualResetEvent();

    protected override IAsyncResetEvent CreateAsyncResetEvent(bool initialState) => new AsyncManualResetEvent(initialState);
}
