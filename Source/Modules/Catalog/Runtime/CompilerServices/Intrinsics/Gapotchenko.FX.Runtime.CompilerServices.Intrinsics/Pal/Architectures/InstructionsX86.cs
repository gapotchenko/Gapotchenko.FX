// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class InstructionsX86
{
    public const byte PushRegister = 0x50;
    public const byte PushRegisterLast = PushRegister + 7;
    public const byte PushImmediate32 = 0x68;
    public const byte PushImmediate8 = 0x6a;
    public const byte PushFlags = 0x9c;
    public const byte Enter = 0xc8;

    public const byte Group1Immediate32 = 0x81;
    public const byte Group1Immediate8 = 0x83;
    public const byte MovRmRegister = 0x89;
    public const byte MovRegisterRm = 0x8b;
    public const byte Lea = 0x8d;

    public const byte Ret = 0xc3;
    public const int JmpRel32 = 0xe9;
    public const int JmpRel32Size = 5;

    public const int RegisterSp = 4;
    public const int RegisterBp = 5;
    public const int ExtendedRegisterOffset = 8;
}
