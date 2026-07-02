// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Diagnostics;

namespace Gapotchenko.FX.Security.Cryptography;

sealed class TripleDESManagedTransform : ManagedBlockTransform
{
    public TripleDESManagedTransform(ReadOnlySpan<byte> key, bool encrypting) :
        base(BlockSize)
    {
        Debug.Assert(key.Length is 16 or 24);

        var kernel1 = new DESManagedTransformKernel(key[..8], encrypting);
        m_Kernel2 = new(key[8..16], !encrypting);
        var kernel3 = key.Length is 16 ? kernel1 : new(key[16..24], encrypting);

        (m_Kernel1, m_Kernel3) = encrypting ? (kernel1, kernel3) : (kernel3, kernel1);
    }

    public override void Dispose()
    {
        m_Kernel1.Dispose();
        m_Kernel2.Dispose();
        m_Kernel3.Dispose();

        base.Dispose();
    }

    protected override void TransformBlockCore(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Span<byte> buffer = stackalloc byte[BlockSize];

        m_Kernel1.Transform(input, buffer);
        m_Kernel2.Transform(buffer, buffer);
        m_Kernel3.Transform(buffer, output);
    }

    const int BlockSize = 8;

    readonly DESManagedTransformKernel m_Kernel1, m_Kernel2, m_Kernel3;
}
