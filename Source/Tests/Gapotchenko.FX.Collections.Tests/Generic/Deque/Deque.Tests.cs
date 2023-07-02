// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Bench;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

public abstract partial class Deque_Tests<T> : IList_Generic_Tests<T>
{
    protected override IList<T> GenericIListFactory() => new Deque<T>();

    protected abstract override T CreateT(int seed);

    // This class is partial.
    // For more code, please take a look at the neighboring source files.
}
