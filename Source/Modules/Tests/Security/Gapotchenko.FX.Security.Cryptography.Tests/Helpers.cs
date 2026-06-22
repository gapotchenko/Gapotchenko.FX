// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
// Portions © Mono Project
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography.Tests;

static class Helpers
{
    public static byte[] Transform(this ICryptoTransform transform, byte[] input, int blockSizeMultiplier = 1)
    {
        var output = new List<byte>(input.Length);
        int blockSize = transform.InputBlockSize * blockSizeMultiplier;
        for (int i = 0; i <= input.Length; i += blockSize)
        {
            int count = Math.Min(blockSize, input.Length - i);
            if (count >= blockSize)
            {
                byte[] buffer = new byte[blockSize];
                int numBytesWritten = transform.TransformBlock(input, i, count, buffer, 0);
                Array.Resize(ref buffer, numBytesWritten);
                output.AddRange(buffer);
            }
            else
            {
                byte[] finalBlock = transform.TransformFinalBlock(input, i, count);
                output.AddRange(finalBlock);
                break;
            }
        }

        return [.. output];
    }
}
