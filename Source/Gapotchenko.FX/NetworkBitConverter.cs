using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// <para>
    /// Converts base data types to an array of bytes, and an array of bytes to base data types in network byte order.
    /// The network byte order is a more widespread name of a Big Endian byte order.
    /// </para>
    /// <seealso cref="LittleEndianBitConverter"/>
    /// </summary>
    public sealed class NetworkBitConverter : IBitConverter
    {
        private NetworkBitConverter()
        {
        }

        /// <summary>
        /// Returns the specified 32-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        public static byte[] GetBytes(int value)
        {
            var buffer = new byte[4];
            FillBytes(value, buffer, 0);
            return buffer;
        }

        /// <summary>
        /// Fills the array with four bytes of the specified 32-bit signed integer value beginning at <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
        public static void FillBytes(int value, byte[] buffer, int startIndex)
        {
            buffer[startIndex++] = (byte)(value >> 24);
            buffer[startIndex++] = (byte)(value >> 16);
            buffer[startIndex++] = (byte)(value >> 8);
            buffer[startIndex] = (byte)value;
        }

        /// <summary>
        /// Fills the array with four bytes of the specified 32-bit signed integer value.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        public static void FillBytes(int value, byte[] buffer) => FillBytes(value, buffer, 0);

        /// <summary>
        /// Returns the specified 32-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        [CLSCompliant(false)]
        public static byte[] GetBytes(uint value)
        {
            var buffer = new byte[4];
            FillBytes(value, buffer, 0);
            return buffer;
        }

        /// <summary>
        /// Fills the array with four bytes of the specified 32-bit unsigned integer value beginning at <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
        [CLSCompliant(false)]
        public static void FillBytes(uint value, byte[] buffer, int startIndex) => FillBytes((int)value, buffer, startIndex);

        /// <summary>
        /// Fills the array with four bytes of the specified 32-bit unsigned integer value.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        [CLSCompliant(false)]
        public static void FillBytes(uint value, byte[] buffer) => FillBytes(value, buffer, 0);

        /// <summary>
        /// Returns the specified 16-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        public static byte[] GetBytes(short value)
        {
            var buffer = new byte[2];
            FillBytes(value, buffer, 0);
            return buffer;
        }

        /// <summary>
        /// Fills the array with two bytes of the specified 16-bit signed integer value beginning at <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
        public static void FillBytes(short value, byte[] buffer, int startIndex)
        {
            buffer[startIndex++] = (byte)(value >> 8);
            buffer[startIndex] = (byte)value;
        }

        /// <summary>
        /// Fills the array with two bytes of the specified 16-bit signed integer value.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        public static void FillBytes(short value, byte[] buffer) => FillBytes(value, buffer, 0);

        /// <summary>
        /// Returns the specified 16-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        [CLSCompliant(false)]
        public static byte[] GetBytes(ushort value)
        {
            var buffer = new byte[2];
            FillBytes(value, buffer, 0);
            return buffer;
        }

        /// <summary>
        /// Fills the array with two bytes of the specified 16-bit unsigned integer value beginning at <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
        [CLSCompliant(false)]
        public static void FillBytes(ushort value, byte[] buffer, int startIndex) => FillBytes((short)value, buffer, startIndex);

        /// <summary>
        /// Fills the array with two bytes of the specified 16-bit unsigned integer value.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        [CLSCompliant(false)]
        public static void FillBytes(ushort value, byte[] buffer) => FillBytes(value, buffer, 0);

        /// <summary>
        /// Returns the specified 64-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        public static byte[] GetBytes(long value)
        {
            var buffer = new byte[8];
            FillBytes(value, buffer, 0);
            return buffer;
        }

        /// <summary>
        /// Fills the array with eight bytes of the specified 64-bit signed integer value beginning at <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
        public static void FillBytes(long value, byte[] buffer, int startIndex)
        {
            FillBytes((int)(value >> 32), buffer, startIndex);
            FillBytes((int)value, buffer, startIndex + 4);
        }

        /// <summary>
        /// Fills the array with eight bytes of the specified 64-bit signed integer value.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        public static void FillBytes(long value, byte[] buffer) => FillBytes(value, buffer, 0);

        /// <summary>
        /// Returns the specified 64-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        [CLSCompliant(false)]
        public static byte[] GetBytes(ulong value)
        {
            var buffer = new byte[8];
            FillBytes(value, buffer, 0);
            return buffer;
        }

        /// <summary>
        /// Fills the array with eight bytes of the specified 64-bit unsigned integer value beginning at <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
        [CLSCompliant(false)]
        public static void FillBytes(ulong value, byte[] buffer, int startIndex) => FillBytes((long)value, buffer, startIndex);

        /// <summary>
        /// Fills the array with eight bytes of the specified 64-bit unsigned integer value.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        [CLSCompliant(false)]
        public static void FillBytes(ulong value, byte[] buffer) => FillBytes(value, buffer, 0);

        /// <summary>
        /// Returns the specified <see cref="bool"/> value as an array of bytes.
        /// </summary>
        /// <param name="value">A <see cref="bool"/> value.</param>
        /// <returns>An array of bytes with length 1.</returns>
        public static byte[] GetBytes(bool value)
        {
            var buffer = new byte[1];
            FillBytes(value, buffer, 0);
            return buffer;
        }

        /// <summary>
        /// Fills the array with one byte of the specified <see cref="bool"/> value beginning at <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="value">A <see cref="bool"/> value.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
        public static void FillBytes(bool value, byte[] buffer, int startIndex)
        {
            buffer[startIndex] = value ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Fills the array with one byte of the specified <see cref="bool"/> value beginning.
        /// </summary>
        /// <param name="value">A <see cref="bool"/> value.</param>
        /// <param name="buffer">The array of bytes to store converted value at.</param>
        public static void FillBytes(bool value, byte[] buffer) => FillBytes(value, buffer, 0);

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified
        /// position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 32-bit signed integer formed by four bytes beginning at startIndex.</returns>
        public static int ToInt32(byte[] value, int startIndex) => (int)ToUInt32(value, startIndex);

        /// <summary>
        /// Returns a 32-bit signed integer converted from the first four bytes of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <returns>A 32-bit signed integer formed by the first four bytes of a byte array.</returns>
        public static int ToInt32(byte[] value) => ToInt32(value, 0);

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified
        /// position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 32-bit unsigned integer formed by four bytes beginning at startIndex.</returns>
        [CLSCompliant(false)]
        public static uint ToUInt32(byte[] value, int startIndex)
        {
            uint x = ToUInt16(value, startIndex);
            x <<= 16;
            x |= ToUInt16(value, startIndex + 2);
            return x;
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from the first four bytes of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <returns>A 32-bit unsigned integer formed by the first four bytes of a byte array.</returns>
        [CLSCompliant(false)]
        public static uint ToUInt32(byte[] value) => ToUInt32(value, 0);

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified
        /// position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
        public static short ToInt16(byte[] value, int startIndex)
        {
            byte b1 = value[startIndex++];
            byte b0 = value[startIndex];
            return (short)((b1 << 8) | b0);
        }

        /// <summary>
        /// Returns a 16-bit signed integer converted from the first two bytes of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <returns>A 16-bit signed integer formed by the first two bytes of a byte array.</returns>
        public static short ToInt16(byte[] value) => ToInt16(value, 0);

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from four bytes at a specified
        /// position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit unsigned integer formed by two bytes beginning at startIndex.</returns>
        [CLSCompliant(false)]
        public static ushort ToUInt16(byte[] value, int startIndex) => (ushort)ToInt16(value, startIndex);

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from the first two bytes of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <returns>A 16-bit unsigned integer formed by the first two bytes of a byte array.</returns>
        [CLSCompliant(false)]
        public static ushort ToUInt16(byte[] value) => ToUInt16(value, 0);

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified
        /// position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 64-bit signed integer formed by eight bytes beginning at startIndex.</returns>
        public static long ToInt64(byte[] value, int startIndex)
        {
            uint h = ToUInt32(value, startIndex);
            uint l = ToUInt32(value, startIndex + 4);
            return (((long)h) << 32) | l;
        }

        /// <summary>
        /// Returns a 64-bit signed integer converted from the first eight bytes of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <returns>A 64-bit signed integer formed by the first eight bytes of a byte array.</returns>
        public static long ToInt64(byte[] value) => ToInt64(value, 0);

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified
        /// position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 64-bit unsigned integer formed by eight bytes beginning at startIndex.</returns>
        [CLSCompliant(false)]
        public static ulong ToUInt64(byte[] value, int startIndex) => (ulong)ToInt64(value, startIndex);

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from the first eight bytes of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <returns>A 64-bit unsigned integer formed by the first eight bytes of a byte array.</returns>
        [CLSCompliant(false)]
        public static ulong ToUInt64(byte[] value) => ToUInt64(value, 0);

        /// <summary>
        /// Returns a Boolean value converted from one byte at a specified position in
        /// a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns><c>true</c> if the byte at startIndex in value is nonzero; otherwise, <c>false</c>.</returns>
        public static bool ToBoolean(byte[] value, int startIndex) => value[startIndex] != 0;

        /// <summary>
        /// Returns a Boolean value converted from the first byte of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <returns><c>true</c> if the the first byte of a byte array is nonzero; otherwise, <c>false</c>.</returns>
        public static bool ToBoolean(byte[] value) => ToBoolean(value, 0);

        #region IBitConverter Implementation

        void IBitConverter.FillBytes(short value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

        void IBitConverter.FillBytes(ushort value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

        void IBitConverter.FillBytes(int value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

        void IBitConverter.FillBytes(uint value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

        void IBitConverter.FillBytes(long value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

        void IBitConverter.FillBytes(ulong value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

        void IBitConverter.FillBytes(bool value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

        void IBitConverter.FillBytes(short value, byte[] buffer) => FillBytes(value, buffer);

        void IBitConverter.FillBytes(ushort value, byte[] buffer) => FillBytes(value, buffer);

        void IBitConverter.FillBytes(int value, byte[] buffer) => FillBytes(value, buffer);

        void IBitConverter.FillBytes(uint value, byte[] buffer) => FillBytes(value, buffer);

        void IBitConverter.FillBytes(long value, byte[] buffer) => FillBytes(value, buffer);

        void IBitConverter.FillBytes(ulong value, byte[] buffer) => FillBytes(value, buffer);

        void IBitConverter.FillBytes(bool value, byte[] buffer) => FillBytes(value, buffer);

        byte[] IBitConverter.GetBytes(short value) => GetBytes(value);

        byte[] IBitConverter.GetBytes(ushort value) => GetBytes(value);

        byte[] IBitConverter.GetBytes(int value) => GetBytes(value);

        byte[] IBitConverter.GetBytes(uint value) => GetBytes(value);

        byte[] IBitConverter.GetBytes(long value) => GetBytes(value);

        byte[] IBitConverter.GetBytes(ulong value) => GetBytes(value);

        byte[] IBitConverter.GetBytes(bool value) => GetBytes(value);

        ushort IBitConverter.ToUInt16(byte[] value, int startIndex) => ToUInt16(value, startIndex);

        short IBitConverter.ToInt16(byte[] value, int startIndex) => ToInt16(value, startIndex);

        int IBitConverter.ToInt32(byte[] value, int startIndex) => ToInt32(value, startIndex);

        uint IBitConverter.ToUInt32(byte[] value, int startIndex) => ToUInt32(value, startIndex);

        long IBitConverter.ToInt64(byte[] value, int startIndex) => ToInt64(value, startIndex);

        ulong IBitConverter.ToUInt64(byte[] value, int startIndex) => ToUInt64(value, startIndex);

        bool IBitConverter.ToBoolean(byte[] value, int startIndex) => ToBoolean(value, startIndex);

        short IBitConverter.ToInt16(byte[] value) => ToInt16(value);

        int IBitConverter.ToInt32(byte[] value) => ToInt32(value);

        long IBitConverter.ToInt64(byte[] value) => ToInt64(value);

        ushort IBitConverter.ToUInt16(byte[] value) => ToUInt16(value);

        uint IBitConverter.ToUInt32(byte[] value) => ToUInt32(value);

        ulong IBitConverter.ToUInt64(byte[] value) => ToUInt64(value);

        bool IBitConverter.ToBoolean(byte[] value) => ToBoolean(value);

        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static IBitConverter _Instance;

        /// <summary>
        /// Gets <see cref="NetworkBitConverter"/> instance.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [CLSCompliant(false)]
        public static IBitConverter Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new NetworkBitConverter();
                return _Instance;
            }
        }
    }
}
