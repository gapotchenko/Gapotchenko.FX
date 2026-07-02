// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography;

sealed class DESManagedTransform(byte[] key, bool encrypting) : ManagedBlockTransform(8)
{
    public override void Dispose()
    {
        m_Kernel.Dispose();

        base.Dispose();
    }

    protected override void TransformBlockCore(ReadOnlySpan<byte> input, Span<byte> output)
    {
        m_Kernel.Transform(input, output);
    }

    readonly DESManagedTransformKernel m_Kernel = new(key, encrypting);
}
