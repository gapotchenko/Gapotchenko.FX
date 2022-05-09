using System;

namespace Gapotchenko.FX.Numerics
{
    /// <summary>
    /// Provides extended methods for intrinsic bit-twiddling operations.
    /// </summary>
    public static class BitOperationsEx
    {
        /// <summary>
        /// Reverses the bits of a specified value.
        /// </summary>
        /// <param name="value">The value whose bits are to be reversed.</param>
        /// <returns>The value with reversed bits.</returns>
        public static byte Reverse(byte value)
        {
            uint u = (uint)value * 0x00020202;
            uint m = 0x01044010;
            uint s = u & m;
            uint t = (u << 2) & (m << 1);
            return (byte)((0x01001001 * (s + t)) >> 24);
        }

        /// <summary>
        /// Reverses the bits of a specified value.
        /// </summary>
        /// <param name="value">The value whose bits are to be reversed.</param>
        /// <returns>The value with reversed bits.</returns>
        [CLSCompliant(false)]
        public static ushort Reverse(ushort value)
        {
            ushort v = value;
            v = (ushort)((v & 0x5555) << 1 | (v >> 1) & 0x5555);
            v = (ushort)((v & 0x3333) << 2 | (v >> 2) & 0x3333);
            v = (ushort)((v & 0x0F0F) << 4 | (v >> 4) & 0x0F0F);
            v = (ushort)((v & 0x00FF) << 8 | (v >> 8) & 0x00FF);
            return v;
        }

        /// <summary>
        /// Reverses the bits of a specified value.
        /// </summary>
        /// <param name="value">The value whose bits are to be reversed.</param>
        /// <returns>The value with reversed bits.</returns>
        [CLSCompliant(false)]
        public static uint Reverse(uint value)
        {
            uint v = value;
            v = (v & 0x55555555) << 1 | (v >> 1) & 0x55555555;
            v = (v & 0x33333333) << 2 | (v >> 2) & 0x33333333;
            v = (v & 0x0F0F0F0F) << 4 | (v >> 4) & 0x0F0F0F0F;
            v = (v << 24) | ((v & 0xFF00) << 8) | ((v >> 8) & 0xFF00) | (v >> 24);
            return v;
        }
    }
}
