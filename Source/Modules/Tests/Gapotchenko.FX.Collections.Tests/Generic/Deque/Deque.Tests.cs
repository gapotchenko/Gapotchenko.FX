// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © Stephen Cleary
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Bench;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

[Trait("Category", "Deque")]
public abstract partial class Deque_Tests<T> : IList_Generic_Tests<T>
{
    // This type is partial.
    // For the rest of the implementation, please take a look at the neighboring source files.

    protected override IList<T> GenericIListFactory() => new Deque<T>();

    protected abstract override T CreateT(int seed);
}
