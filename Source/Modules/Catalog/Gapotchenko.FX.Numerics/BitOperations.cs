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
    static BitOperations() => Intrinsics.InitializeType(typeof(BitOperations));

    // Some routines use techniques from the "Bit Twiddling Hacks" by Sean Eron Anderson:
    // http://graphics.stanford.edu/~seander/bithacks.html

    static readonly int[] m_Log2DeBruijn32 =
    [
         0,  9,  1, 10, 13, 21,  2, 29,
        11, 14, 16, 18, 22, 25,  3, 30,
         8, 12, 20, 28, 15, 17, 24,  7,
        19, 27, 23,  6, 26,  5,  4, 31
    ];

    /// <summary>
    /// Returns the integer (floor) base 2 logarithm of a specified number.
    /// </summary>
    /// <remarks>
    /// Log2(0) returns an undefined value since such operation is undefined.
    /// The behavior corresponds to <c>BSR</c> instruction from Intel x86 instruction set.
    /// </remarks>
    /// <param name="value">A number whose integer (floor) base 2 logarithm is to be found.</param>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    [MachineCodeIntrinsic(Architecture.X64, 0x0f, 0xbd, 0xc1)]  // BSR EAX, ECX
    public static int Log2(uint value)
    {
        // Round down to one less than a power of 2.
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        var index = (value * 0x07C4ACDDU) >> 27;
        return m_Log2DeBruijn32[index];
    }

    /// <summary>
    /// Returns the bit population count for a specified value.
    /// The result corresponds to the number of bits set to <c>1</c>.
    /// </summary>
    /// <param name="value">The value.</param>
    [CLSCompliant(false)]
    public static int PopCount(uint value)
    {
        // TODO: Implement POPCNT machine code intrinsic for x64.
        // https://en.wikipedia.org/wiki/SSE4#POPCNT_and_LZCNT

        var x = value;
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
    public static int PopCount(ulong value)
    {
        const ulong Mask01010101 = 0x5555555555555555UL;
        const ulong Mask00110011 = 0x3333333333333333UL;
        const ulong Mask00001111 = 0x0F0F0F0F0F0F0F0FUL;
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
