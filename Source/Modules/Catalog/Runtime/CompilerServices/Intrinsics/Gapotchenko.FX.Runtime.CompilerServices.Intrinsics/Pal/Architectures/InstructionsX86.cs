// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class InstructionsX86
{
    public const byte Ret = 0xc3;
    public const int JmpRel32 = 0xe9;
    public const int JmpRel32Size = 5;
}
