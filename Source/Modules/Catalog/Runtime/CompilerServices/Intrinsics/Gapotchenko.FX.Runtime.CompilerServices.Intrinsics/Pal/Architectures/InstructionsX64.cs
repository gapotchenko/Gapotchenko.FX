// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class InstructionsX64 : InstructionsX86
{
    public static ReadOnlySpan<byte> JmpAbs64 => [0xff, 0x25, 0x00, 0x00, 0x00, 0x00];
    public const int JmpAbs64Size = 6 + 8;
}
