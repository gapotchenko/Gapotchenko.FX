// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.x86;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.x64;

abstract class InstructionsX64 : InstructionsX86
{
    public const byte RexB = 0x41;
    public const byte RexW = 0x48;

    public const byte ModRmAddRsp = 0xc4;
    public const byte ModRmAndRsp = 0xe4;
    public const byte ModRmSubRsp = 0xec;
    public const byte ModRmMovRbpRsp = 0xec;
    public const byte ModRmMovRspRbp = 0xe5;
    public const byte ModRmLeaRbpRspDisp8 = 0x6c;
    public const byte ModRmLeaRbpRspDisp32 = 0xac;
    public const byte SibRsp = 0x24;

    public static ReadOnlySpan<byte> JmpAbs64 => [0xff, 0x25, 0x00, 0x00, 0x00, 0x00];
    public const int JmpAbs64Size = 6 + 8;
}
