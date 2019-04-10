using Gapotchenko.FX.Runtime.CompilerServices;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics
{
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
        static BitOperations() => IntrinsicServices.InitializeType(typeof(BitOperations));

        // Some routines use techniques from the "Bit Twiddling Hacks" by Sean Eron Anderson:
        // http://graphics.stanford.edu/~seander/bithacks.html

        static readonly int[] _Log2DeBruijn32 =
        {
             0,  9,  1, 10, 13, 21,  2, 29,
            11, 14, 16, 18, 22, 25,  3, 30,
             8, 12, 20, 28, 15, 17, 24,  7,
            19, 27, 23,  6, 26,  5,  4, 31
        };

        /// <summary>
        /// Returns the integer (floor) base 2 logarithm of a specified number.
        /// By convention, input value 0 returns 0 since log2(0) is undefined.
        /// The behavior corresponds to <c>BSR</c> instruction from Intel x86 instruction set.
        /// </summary>
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
            return _Log2DeBruijn32[index];
        }

        /// <summary>
        /// Returns the bit population count for a specified value.
        /// It corresponds to the number of bits set to 1.
        /// The behavior corresponds to <c>POPCNT</c> instruction from Intel x86 instruction set.
        /// </summary>
        /// <param name="value">The value.</param>
        [CLSCompliant(false)]
        public static int PopCount(uint value)
        {
            // TODO: Implement POPCNT hardware intrinsic.
            // https://en.wikipedia.org/wiki/SSE4#POPCNT_and_LZCNT

            var x = value;
            x = x - ((x >> 1) & 0x55555555);
            x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
            x = (x + (x >> 4)) & 0x0f0f0f0f;
            x += x >> 8;
            x += x >> 16;
            return (int)(x & 0x3f);
        }

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// The behavior corresponds to <c>ROL</c> instruction from Intel x86 instruction set.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">
        /// The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32.
        /// </param>
        /// <returns>The rotated value.</returns>
#if TFF_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static uint RotateLeft(uint value, int offset) => (value << offset) | (value >> (32 - offset));

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// The behavior corresponds to <c>ROL</c> instruction from Intel x86 instruction set.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">
        /// The number of bits to rotate by.
        /// Any value outside the range [0..63] is treated as congruent mod 64.
        /// </param>
        /// <returns>The rotated value.</returns>
#if TFF_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static ulong RotateLeft(ulong value, int offset) => (value << offset) | (value >> (64 - offset));

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// The behavior corresponds to <c>ROR</c> instruction from Intel x86 instruction set.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">
        /// The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32.
        /// </param>
        /// <returns>The rotated value.</returns>
#if TFF_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static uint RotateRight(uint value, int offset) => (value >> offset) | (value << (32 - offset));

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// The behavior corresponds to <c>ROR</c> instruction from Intel x86 instruction set.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">
        /// The number of bits to rotate by.
        /// Any value outside the range [0..63] is treated as congruent mod 64.
        /// </param>
        /// <returns>The rotated value.</returns>
#if TFF_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static ulong RotateRight(ulong value, int offset) => (value >> offset) | (value << (64 - offset));
    }
}
