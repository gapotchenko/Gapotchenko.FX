// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class AdapterX64 : AdapterX86
{
    protected static ReadOnlySpan<byte> JmpAbs64 => [0xff, 0x25, 0x00, 0x00, 0x00, 0x00];
    protected const int JmpAbs64Size = 6 + 8;
}
