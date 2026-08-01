// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

static class InstructionsArm32
{
    public const nint BranchMaximumDistance = 1 << 24;

    public const ushort BxLr = 0x4770;

    public static bool IsPushRegisters(ushort instruction) =>
        (instruction & PushRegistersMask) == PushRegisters;

    public static bool IsPushRegistersWithLinkRegister(ushort instruction) =>
        (instruction & PushRegistersWithLinkRegisterMask) == PushRegistersWithLinkRegister;

    public static bool IsPushRegistersWide(ReadOnlySpan<ushort> instructions) =>
        instructions.Length >= 2 &&
        instructions[0] == PushRegistersWide &&
        instructions[1] != 0;

    public static bool IsPushRegistersWideWithLinkRegister(ReadOnlySpan<ushort> instructions) =>
        IsPushRegistersWide(instructions) &&
        (instructions[1] & LinkRegisterMask) != 0;

    public static bool IsStackAllocation(ushort instruction) =>
        (instruction & StackAllocationMask) == StackAllocation;

    const ushort PushRegisters = 0xb400;
    const ushort PushRegistersMask = 0xfe00;
    const ushort PushRegistersWithLinkRegister = 0xb500;
    const ushort PushRegistersWithLinkRegisterMask = 0xff00;
    const ushort PushRegistersWide = 0xe92d;
    const ushort LinkRegisterMask = 0x4000;
    const ushort StackAllocation = 0xb080;
    const ushort StackAllocationMask = 0xff80;
}
