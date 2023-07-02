// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

public sealed class Deque_Tests_String : Deque_Tests<string>
{
    protected override string CreateT(int seed) => CreateString(new Random(seed));

    static string CreateString(Random random)
    {
        int length = random.Next(5, 15);
        var bytes = new byte[length];
        random.NextBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}

public sealed class Deque_Tests_Int32 : Deque_Tests<int>
{
    protected override int CreateT(int seed) => CreateInt32(new Random(seed));

    static int CreateInt32(Random random) => random.Next();
}
