// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Security.Cryptography;

static class Utils
{
    public static byte[] GenerateRandomBytes(int count)
    {
        byte[] bytes = new byte[count];

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        RandomNumberGenerator.Fill(bytes);
#else
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        rng.Dispose();
#endif

        return bytes;
    }
}
