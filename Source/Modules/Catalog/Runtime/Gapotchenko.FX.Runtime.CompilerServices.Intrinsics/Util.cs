// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices;

static class Util
{
    public static bool HasPrologue(ReadOnlySpan<byte> data, byte[][] prologues)
    {
        foreach (byte[] prologue in prologues)
        {
            if (data.StartsWith(prologue))
                return true;
        }
        return false;
    }
}
