// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

static class NormalOperations
{
    static NormalOperations()
    {
        Intrinsics.InitializeType(typeof(NormalOperations));
    }

    /// <summary>
    /// Returns the integer (floor) base 2 logarithm of a specified number.
    /// </summary>
    /// <remarks>
    /// By convention, input value <c>0</c> returns <c>0</c> since <c>Log2(0)</c> is undefined.
    /// </remarks>
    /// <param name="value">A number whose integer (floor) base 2 logarithm is to be found.</param>
    [MachineCodeIntrinsic(
        Architecture.X86,
        // The 0 -> 0 contract is fulfilled by setting the LSB to 1.
        // Log2(1) is 0, and setting the LSB for values > 1 does not change the log2 result.
        0x83, 0xc9, 0x01,  // OR ECX,1
        0x0f, 0xbd, 0xc1,  // BSR EAX,ECX
        AdditionalArchitectures = [Architecture.X64],
        SupportedOSPlatforms = ["windows"])]
    [MachineCodeIntrinsic(
        Architecture.X86,
        0x8b, 0x44, 0x24, 0x04,  // MOV EAX,[ESP+4]
        0x83, 0xc8, 0x01,        // OR EAX,1
        0x0f, 0xbd, 0xc0,        // BSR EAX,EAX
        SupportedOSPlatforms = ["linux"])]
    [MachineCodeIntrinsic(
        Architecture.X64,
        0x83, 0xcf, 0x01,  // OR EDI,1
        0x0f, 0xbd, 0xc7,  // BSR EAX,EDI
        SupportedOSPlatforms = ["linux", "macos"])]
    [MachineCodeIntrinsic(
        Architecture.Arm64,
        0x00, 0x00, 0x00, 0x32,   // ORR W0,W0,#1
        0x00, 0x10, 0xc0, 0x5a,   // CLZ W0,W0
        0x00, 0x10, 0x00, 0x52,   // EOR W0,W0,#31
        SupportedOSPlatforms = ["windows", "linux", "macos"])]
    [MachineCodeIntrinsic(
        Architecture.Arm,
        0x01, 0x21,              // MOVS R1,1
        0x08, 0x43,              // ORRS R0,R1
        0xb0, 0xfa, 0x80, 0xf0,  // CLZ R0,R0
        0x1f, 0x21,              // MOVS R1,31
        0x48, 0x40,              // EORS R0,R1
        SupportedOSPlatforms = ["linux"])]
    [MethodImpl(Intrinsics.MethodImplOptions)]
    public static int Log2_Intrinsic(uint value)
    {
        // Round down to one less than a power of 2.
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        uint index = (value * 0x07c4acddU) >> 27;
        return m_Log2DeBruijn32[index];
    }

    static readonly int[] m_Log2DeBruijn32 =
    [
         0,  9,  1, 10, 13, 21,  2, 29,
        11, 14, 16, 18, 22, 25,  3, 30,
         8, 12, 20, 28, 15, 17, 24,  7,
        19, 27, 23,  6, 26,  5,  4, 31
    ];
}
