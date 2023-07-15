// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests;

[TestClass]
[TestCategory("auto")]
public sealed class AsyncAutoResetEventTests : AsyncResetEventTestsBase
{
    protected override bool IsAutoReset => true;

    protected override IAsyncResetEvent CreateAsyncResetEvent(bool initialState) => new AsyncAutoResetEvent(initialState);
}
