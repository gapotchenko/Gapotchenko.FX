// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Tests.Utils;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

public sealed class Deque_Tests_String : Deque_Tests<string>
{
    protected override string CreateT(int seed) => TestData.CreateString(new Random(seed));
}

public sealed class Deque_Tests_Int32 : Deque_Tests<int>
{
    protected override int CreateT(int seed) => TestData.CreateInt32(new Random(seed));
}
