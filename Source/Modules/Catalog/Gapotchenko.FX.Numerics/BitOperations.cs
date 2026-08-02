// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © Sean Eron Anderson
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Runtime.CompilerServices;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !TFF_BITOPERATIONS

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Numerics;

/// <summary>
/// <para>
/// Provides methods for intrinsic bit-twiddling operations.
/// The methods use hardware intrinsics of a host platform when available; otherwise, they use optimized software fallbacks.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
public static class BitOperations
{
    static BitOperations()
    {
        Intrinsics.InitializeType(typeof(BitOperations));
    }

    // Some routines use techniques from the "Bit Twiddling Hacks" by Sean Eron Anderson:
    // http://graphics.stanford.edu/~seander/bithacks.html

    /// <inheritdoc cref="Log2(uint)"/>
    [CLSCompliant(false)]
    [MachineCodeIntrinsic(
        Architecture.X64,
        0x48, 0x83, 0xc9, 0x01,   // OR RCX,1
        0x48, 0x0f, 0xbd, 0xc1,   // BSR RAX,RCX
        SupportedOSPlatforms = ["windows"])]
    [MachineCodeIntrinsic(
        Architecture.X64,
        0x48, 0x83, 0xc9, 0x01,        // OR RCX,1
        0xf3, 0x48, 0x0f, 0xbd, 0xc1,  // LZCNT RAX,RCX
        0x48, 0x83, 0xf0, 0x3f,        // XOR RAX,63
        RequiredFeatures = [MachineCodeIntrinsicFeature.Lzcnt],
        SupportedOSPlatforms = ["windows"],
        Priority = 10)]               // LZCNT is faster than BSR on AMD processors
    [MachineCodeIntrinsic(
        Architecture.Arm64,
        0x00, 0x00, 0x40, 0xb2,   // ORR X0,X0,#1
        0x00, 0x10, 0xc0, 0xda,   // CLZ X0,X0
        0x00, 0x14, 0x40, 0xd2,   // EOR X0,X0,#63
        SupportedOSPlatforms = ["windows"])]
    [MethodImpl(Intrinsics.MethodImplOptions)]
    public static int Log2(ulong value)
    {
        uint hi = (uint)(value >> 32);
        if (hi == 0)
            return Log2((uint)value);
        else
            return 32 + Log2(hi);
    }

    /// <summary>
    /// Returns the integer (floor) base 2 logarithm of a specified number.
    /// </summary>
    /// <remarks>
    /// By convention, input value <c>0</c> returns <c>0</c> since <c>Log2(0)</c> is undefined.
    /// </remarks>
    /// <param name="value">A number whose integer (floor) base 2 logarithm is to be found.</param>
    [CLSCompliant(false)]
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
        0x83, 0xc9, 0x01,        // OR ECX,1
        0xf3, 0x0f, 0xbd, 0xc1,  // LZCNT EAX,ECX
        0x83, 0xf0, 0x1f,        // XOR EAX,31
        AdditionalArchitectures = [Architecture.X64],
        RequiredFeatures = [MachineCodeIntrinsicFeature.Lzcnt],
        SupportedOSPlatforms = ["windows"],
        Priority = 10)]         // LZCNT is faster than BSR on AMD processors
    [MachineCodeIntrinsic(
        Architecture.Arm,
        0x01, 0x21,              // MOVS R1,1
        0x08, 0x43,              // ORRS R0,R1
        0xb0, 0xfa, 0x80, 0xf0,  // CLZ R0,R0
        0x1f, 0x21,              // MOVS R1,31
        0x48, 0x40,              // EORS R0,R1
        SupportedOSPlatforms = ["linux"])]
    [MachineCodeIntrinsic(
        Architecture.Arm64,
        0x00, 0x00, 0x00, 0x32,   // ORR W0,W0,#1
        0x00, 0x10, 0xc0, 0x5a,   // CLZ W0,W0
        0x00, 0x10, 0x00, 0x52,   // EOR W0,W0,#31
        SupportedOSPlatforms = ["windows"])]
    [MethodImpl(Intrinsics.MethodImplOptions)]
    public static int Log2(uint value)
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

    /// <summary>
    /// Counts the number of leading zero bits in an unsigned 32-bit integer mask.
    /// </summary>
    /// <param name="value">The mask.</param>
    /// <returns>The number of leading zero bits in a mask.</returns>
    [CLSCompliant(false)]
    [MachineCodeIntrinsic(
        Architecture.X86,
        0xf3, 0x0f, 0xbd, 0xc1,  // LZCNT EAX,ECX
        AdditionalArchitectures = [Architecture.X64],
        RequiredFeatures = [MachineCodeIntrinsicFeature.Lzcnt],
        SupportedOSPlatforms = ["windows"])]
    [MachineCodeIntrinsic(
        Architecture.Arm64,
        0x00, 0x10, 0xc0, 0x5a,  // CLZ W0,W0
        SupportedOSPlatforms = ["windows"])]
    [MethodImpl(Intrinsics.MethodImplOptions)]
    public static int LeadingZeroCount(uint value)
    {
        return (31 ^ Log2(value)) + (value == 0 ? 1 : 0);
    }

    /// <summary>
    /// Counts the number of leading zero bits in an unsigned 64-bit integer mask.
    /// </summary>
    /// <param name="value">The mask.</param>
    /// <returns>The number of leading zero bits in a mask.</returns>
    [CLSCompliant(false)]
    [MachineCodeIntrinsic(
        Architecture.X64,
        0xf3, 0x48, 0x0f, 0xbd, 0xc1,  // LZCNT RAX,RCX
        RequiredFeatures = [MachineCodeIntrinsicFeature.Lzcnt],
        SupportedOSPlatforms = ["windows"])]
    [MachineCodeIntrinsic(
        Architecture.Arm64,
        0x00, 0x10, 0xc0, 0xda,  // CLZ X0,X0
        SupportedOSPlatforms = ["windows"])]
    [MethodImpl(Intrinsics.MethodImplOptions)]
    public static int LeadingZeroCount(ulong value)
    {
        return (63 ^ Log2(value)) + (value == 0 ? 1 : 0);
    }

    /// <summary>
    /// Returns the bit population count for a specified value.
    /// The result corresponds to the number of bits set to <c>1</c>.
    /// </summary>
    /// <param name="value">The value.</param>
    [CLSCompliant(false)]
    [MachineCodeIntrinsic(
        Architecture.X86,
        0xf3, 0x0f, 0xb8, 0xc1,  // POPCNT EAX,ECX
        AdditionalArchitectures = [Architecture.X64],
        RequiredFeatures = [MachineCodeIntrinsicFeature.Popcnt],
        SupportedOSPlatforms = ["windows"])]
    [MachineCodeIntrinsic(
        Architecture.Arm64,
        0x00, 0x00, 0x27, 0x1e,   // FMOV S0,W0
        0x00, 0x58, 0x20, 0x0e,   // CNT V0.8B,V0.8B
        0x00, 0xb8, 0x31, 0x0e,   // ADDV B0,V0.8B
        0x00, 0x3c, 0x01, 0x0e,   // UMOV W0,V0.B[0]
        RequiredFeatures = [MachineCodeIntrinsicFeature.AdvSimd],
        SupportedOSPlatforms = ["windows"])]
    [MethodImpl(Intrinsics.MethodImplOptions)]
    public static int PopCount(uint value)
    {
        uint x = value;
        x -= (x >> 1) & 0x55555555;
        x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
        x = (x + (x >> 4)) & 0x0f0f0f0f;
        x += x >> 8;
        x += x >> 16;
        return (int)(x & 0x3f);
    }

    /// <summary>
    /// Returns the bit population count for a specified value.
    /// The result corresponds to the number of bits set to <c>1</c>.
    /// </summary>
    /// <param name="value">The value.</param>
    [CLSCompliant(false)]
    [MachineCodeIntrinsic(
        Architecture.X86,
        0xf3, 0x0f, 0xb8, 0x44, 0x24, 0x04,  // POPCNT EAX,[ESP+4]
        0xf3, 0x0f, 0xb8, 0x4c, 0x24, 0x08,  // POPCNT ECX,[ESP+8]
        0x03, 0xc1,                          // ADD EAX,ECX
        0xc2, 0x08, 0x00,                    // RET 8
        RequiredFeatures = [MachineCodeIntrinsicFeature.Popcnt],
        SupportedOSPlatforms = ["windows"])]
    [MachineCodeIntrinsic(
        Architecture.X64,
        0xf3, 0x48, 0x0f, 0xb8, 0xc1,  // POPCNT RAX,RCX
        RequiredFeatures = [MachineCodeIntrinsicFeature.Popcnt],
        SupportedOSPlatforms = ["windows"])]
    [MachineCodeIntrinsic(
        Architecture.Arm64,
        0x00, 0x00, 0x67, 0x9e,   // FMOV D0,X0
        0x00, 0x58, 0x20, 0x0e,   // CNT V0.8B,V0.8B
        0x00, 0xb8, 0x31, 0x0e,   // ADDV B0,V0.8B
        0x00, 0x3c, 0x01, 0x0e,   // UMOV W0,V0.B[0]
        RequiredFeatures = [MachineCodeIntrinsicFeature.AdvSimd],
        SupportedOSPlatforms = ["windows"])]
    [MethodImpl(Intrinsics.MethodImplOptions)]
    public static int PopCount(ulong value)
    {
        const ulong Mask01010101 = 0x5555555555555555UL;
        const ulong Mask00110011 = 0x3333333333333333UL;
        const ulong Mask00001111 = 0x0f0f0f0f0f0f0f0fUL;
        const ulong Mask00000001 = 0x0101010101010101UL;

        value -= (value >> 1) & Mask01010101;
        value = (value & Mask00110011) + ((value >> 2) & Mask00110011);
        return (int)(unchecked(((value + (value >> 4)) & Mask00001111) * Mask00000001) >> 56);
    }

    /// <summary>
    /// <para>
    /// Rotates the specified value left by the specified number of bits.
    /// </para>
    /// <para>
    /// The behavior corresponds to <c>ROL</c> instruction from Intel x86 instruction set.
    /// </para>
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">
    /// The number of bits to rotate by.
    /// Any value outside the range [0..31] is treated as congruent mod 32.
    /// </param>
    /// <returns>The rotated value.</returns>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateLeft(uint value, int offset) => (value << offset) | (value >> (32 - offset));

    /// <summary>
    /// <para>
    /// Rotates the specified value left by the specified number of bits.
    /// </para>
    /// <para>
    /// The behavior corresponds to <c>ROL</c> instruction from Intel x86 instruction set.
    /// </para>
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">
    /// The number of bits to rotate by.
    /// Any value outside the range [0..63] is treated as congruent mod 64.
    /// </param>
    /// <returns>The rotated value.</returns>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RotateLeft(ulong value, int offset) => (value << offset) | (value >> (64 - offset));

    /// <summary>
    /// <para>
    /// Rotates the specified value right by the specified number of bits.
    /// </para>
    /// <para>
    /// The behavior corresponds to <c>ROR</c> instruction from Intel x86 instruction set.
    /// </para>
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">
    /// The number of bits to rotate by.
    /// Any value outside the range [0..31] is treated as congruent mod 32.
    /// </param>
    /// <returns>The rotated value.</returns>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateRight(uint value, int offset) => (value >> offset) | (value << (32 - offset));

    /// <summary>
    /// <para>
    /// Rotates the specified value right by the specified number of bits.
    /// </para>
    /// <para>
    /// The behavior corresponds to <c>ROR</c> instruction from Intel x86 instruction set.
    /// </para>
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">
    /// The number of bits to rotate by.
    /// Any value outside the range [0..63] is treated as congruent mod 64.
    /// </param>
    /// <returns>The rotated value.</returns>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RotateRight(ulong value, int offset) => (value >> offset) | (value << (64 - offset));
}

#else

[assembly: TypeForwardedTo(typeof(BitOperations))]

#endif
