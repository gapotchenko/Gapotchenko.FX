using System.Diagnostics;

namespace Gapotchenko.FX;

/// <summary>
/// Converts base data types to a span or an array of bytes, and a span of bytes to base data types in little-endian byte order.
/// </summary>
/// <remarks>
/// <seealso cref="BigEndianBitConverter"/> is a big-endian counterpart of <see cref="LittleEndianBitConverter"/>.
/// </remarks>
public sealed class LittleEndianBitConverter : IBitConverter
{
    LittleEndianBitConverter()
    {
    }

    #region Kernel

    static ushort ToUInt16Core(ReadOnlySpan<byte> value) => (ushort)
        (value[0] |
        value[1] << 8);

    static short ToInt16Core(in ReadOnlySpan<byte> value) => (short)ToUInt16Core(value);

    static uint ToUInt32Core(ReadOnlySpan<byte> value) =>
        value[0] |
        (uint)value[1] << 8 |
        (uint)value[2] << 16 |
        (uint)value[3] << 24;

    static int ToInt32Core(in ReadOnlySpan<byte> value) => (int)ToUInt32Core(value);

    static ulong ToUInt64Core(ReadOnlySpan<byte> value) =>
        value[0] |
        (ulong)value[1] << 8 |
        (ulong)value[2] << 16 |
        (ulong)value[3] << 24 |
        (ulong)value[4] << 32 |
        (ulong)value[5] << 40 |
        (ulong)value[6] << 48 |
        (ulong)value[7] << 56;

    static long ToInt64Core(in ReadOnlySpan<byte> value) => (long)ToUInt64Core(value);

    static float ToSingleCore(ReadOnlySpan<byte> value) => BitConverterEx.Int32BitsToSingle(ToInt32Core(value));

    static double ToDoubleCore(ReadOnlySpan<byte> value) => BitConverter.Int64BitsToDouble(ToInt64Core(value));

    static decimal ToDecimalCore(ReadOnlySpan<byte> value)
    {
        const int n = 4;
        var bits = new int[n];

        for (int i = 0; i < n; ++i)
            bits[i] = ToInt32Core(value.Slice(i * sizeof(int)));

        return new decimal(bits);
    }

    #endregion

    /// <summary>
    /// Returns the specified 32-bit signed integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    public static byte[] GetBytes(int value)
    {
        var buffer = new byte[4];
        FillBytes(value, buffer);
        return buffer;
    }

    /// <summary>
    /// Fills the array with four bytes of the specified 32-bit signed integer value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is be stored.</param>
    public static void FillBytes(int value, byte[] buffer, int startIndex)
    {
        BitConverterServices.ValidateFillArguments(buffer, startIndex, 4);

        buffer[startIndex++] = (byte)value;
        buffer[startIndex++] = (byte)(value >> 8);
        buffer[startIndex++] = (byte)(value >> 16);
        buffer[startIndex] = (byte)(value >> 24);
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
        FillBytes(value, buffer);
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
        FillBytes(value, buffer);
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
        BitConverterServices.ValidateFillArguments(buffer, startIndex, 2);

        buffer[startIndex++] = (byte)value;
        buffer[startIndex] = (byte)(value >> 8);
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
        FillBytes(value, buffer);
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
        FillBytes(value, buffer);
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
        BitConverterServices.ValidateFillArguments(buffer, startIndex, 8);

        FillBytes((int)value, buffer, startIndex);
        FillBytes((int)(value >> 32), buffer, startIndex + 4);
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
        FillBytes(value, buffer);
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
    /// Returns the specified single-precision floating point value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    public static byte[] GetBytes(float value)
    {
        var buffer = new byte[4];
        FillBytes(value, buffer);
        return buffer;
    }

    /// <summary>
    /// Fills the array with four bytes of the specified single-precision floating point value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    public static void FillBytes(float value, byte[] buffer, int startIndex)
    {
        BitConverterServices.ValidateFillArguments(buffer, startIndex, 4);

        FillBytes(BitConverterEx.SingleToInt32Bits(value), buffer, startIndex);
    }

    /// <summary>
    /// Fills the array with four bytes of the specified single-precision floating point value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    public static void FillBytes(float value, byte[] buffer) => FillBytes(value, buffer, 0);

    /// <summary>
    /// Returns the specified double-precision floating point value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    public static byte[] GetBytes(double value)
    {
        var buffer = new byte[8];
        FillBytes(value, buffer);
        return buffer;
    }

    /// <summary>
    /// Fills the array with eight bytes of the specified double-precision floating point value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    public static void FillBytes(double value, byte[] buffer, int startIndex)
    {
        BitConverterServices.ValidateFillArguments(buffer, startIndex, 8);

        FillBytes(BitConverter.DoubleToInt64Bits(value), buffer, startIndex);
    }

    /// <summary>
    /// Fills the array with eight bytes of the specified double-precision floating point value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    public static void FillBytes(double value, byte[] buffer) => FillBytes(value, buffer, 0);

    /// <summary>
    /// Returns the specified decimal value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 16.</returns>
    public static byte[] GetBytes(decimal value)
    {
        var buffer = new byte[16];
        FillBytes(value, buffer);
        return buffer;
    }

    /// <summary>
    /// Fills the array with sixteen bytes of the specified decimal value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    public static void FillBytes(decimal value, byte[] buffer, int startIndex)
    {
        BitConverterServices.ValidateFillArguments(buffer, startIndex, 16);

        var bits = decimal.GetBits(value);
        for (int i = 0; i < bits.Length; ++i)
            FillBytes(bits[i], buffer, startIndex + i * sizeof(int));
    }

    /// <summary>
    /// Fills the array with sixteen bytes of the specified decimal value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    public static void FillBytes(decimal value, byte[] buffer) => FillBytes(value, buffer, 0);

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
    public static void FillBytes(bool value, byte[] buffer, int startIndex) => BitConverterServices.FillBytes(value, buffer, startIndex);

    /// <summary>
    /// Fills the array with one byte of the specified <see cref="bool"/> value beginning.
    /// </summary>
    /// <param name="value">A <see cref="bool"/> value.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    public static void FillBytes(bool value, byte[] buffer) => FillBytes(value, buffer, 0);

    /// <summary>
    /// Returns a 16-bit signed integer converted from the first two bytes of a specified span.
    /// </summary>
    /// <param name="value">The byte span.</param>
    /// <returns>A 16-bit signed integer formed by the first two bytes of a specified span.</returns>
    public static short ToInt16(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(short));
        return ToInt16Core(value);
    }

    /// <summary>
    /// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
    public static short ToInt16(byte[] value, int startIndex) =>
        ToInt16Core(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(short)));

    /// <summary>
    /// Returns a 16-bit signed integer converted from the first two bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 16-bit signed integer formed by the first two bytes of a byte array.</returns>
    public static short ToInt16(byte[] value) => ToInt16(value, 0);

    /// <summary>
    /// Returns a 16-bit unsigned integer converted from the first two bytes of a specified span.
    /// </summary>
    /// <param name="value">The byte span.</param>
    /// <returns>A 16-bit unsigned integer formed by the first two bytes of a specified span.</returns>
    [CLSCompliant(false)]
    public static ushort ToUInt16(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(ushort));
        return ToUInt16Core(value);
    }

    /// <summary>
    /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 16-bit unsigned integer formed by two bytes beginning at startIndex.</returns>
    [CLSCompliant(false)]
    public static ushort ToUInt16(byte[] value, int startIndex) =>
        ToUInt16Core(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(ushort)));

    /// <summary>
    /// Returns a 16-bit unsigned integer converted from the first two bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 16-bit unsigned integer formed by the first two bytes of a byte array.</returns>
    [CLSCompliant(false)]
    public static ushort ToUInt16(byte[] value) => ToUInt16(value, 0);

    /// <summary>
    /// Returns a 32-bit signed integer converted from the first four bytes of a specified span.
    /// </summary>
    /// <param name="value">The byte span.</param>
    /// <returns>A 32-bit signed integer formed by the first four bytes of a specified span.</returns>
    public static int ToInt32(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(int));
        return ToInt32Core(value);
    }

    /// <summary>
    /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 32-bit signed integer formed by four bytes beginning at startIndex.</returns>
    public static int ToInt32(byte[] value, int startIndex) =>
        ToInt32Core(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(int)));

    /// <summary>
    /// Returns a 32-bit signed integer converted from the first four bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 32-bit signed integer formed by the first four bytes of a byte array.</returns>
    public static int ToInt32(byte[] value) => ToInt32(value, 0);

    /// <summary>
    /// Returns a 32-bit unsigned integer converted from the first four bytes of a specified span.
    /// </summary>
    /// <param name="value">The byte span.</param>
    /// <returns>A 32-bit unsigned integer formed by the first four bytes of a specified span.</returns>
    [CLSCompliant(false)]
    public static uint ToUInt32(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(uint));
        return ToUInt32Core(value);
    }

    /// <summary>
    /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 32-bit unsigned integer formed by four bytes beginning at startIndex.</returns>
    [CLSCompliant(false)]
    public static uint ToUInt32(byte[] value, int startIndex) =>
        ToUInt32Core(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(uint)));

    /// <summary>
    /// Returns a 32-bit unsigned integer converted from the first four bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 32-bit unsigned integer formed by the first four bytes of a byte array.</returns>
    [CLSCompliant(false)]
    public static uint ToUInt32(byte[] value) => ToUInt32(value, 0);

    /// <summary>
    /// Returns a 64-bit signed integer converted from the first eight bytes of a specified span.
    /// </summary>
    /// <param name="value">The byte span.</param>
    /// <returns>A 64-bit signed integer formed by the first eight bytes of a specified span.</returns>
    public static long ToInt64(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(long));
        return ToInt64Core(value);
    }

    /// <summary>
    /// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 64-bit signed integer formed by eight bytes beginning at startIndex.</returns>
    public static long ToInt64(byte[] value, int startIndex) =>
        ToInt64Core(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(long)));

    /// <summary>
    /// Returns a 64-bit signed integer converted from the first eight bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 64-bit signed integer formed by the first eight bytes of a byte array.</returns>
    public static long ToInt64(byte[] value) => ToInt64(value, 0);

    /// <summary>
    /// Returns a 64-bit unsigned integer converted from the first eight bytes of a specified span.
    /// </summary>
    /// <param name="value">The byte span.</param>
    /// <returns>A 64-bit unsigned integer formed by the first eight bytes of a specified span.</returns>
    [CLSCompliant(false)]
    public static ulong ToUInt64(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(ulong));
        return ToUInt64Core(value);
    }

    /// <summary>
    /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 64-bit unsigned integer formed by eight bytes beginning at startIndex.</returns>
    [CLSCompliant(false)]
    public static ulong ToUInt64(byte[] value, int startIndex) =>
        ToUInt64Core(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(ulong)));

    /// <summary>
    /// Returns a 64-bit unsigned integer converted from the first eight bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 64-bit unsigned integer formed by the first eight bytes of a byte array.</returns>
    [CLSCompliant(false)]
    public static ulong ToUInt64(byte[] value) => ToUInt64(value, 0);

    /// <summary>
    /// Returns a single-precision floating point number converted from four bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A single-precision floating point number formed by four bytes of a specified span.</returns>
    public static float ToSingle(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(float));
        return ToSingleCore(value);
    }

    /// <summary>
    /// Returns a single-precision floating point number converted from four bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A single-precision floating point number formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
    public static float ToSingle(byte[] value, int startIndex) =>
        ToSingleCore(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(float)));

    /// <summary>
    /// Returns a single-precision floating point number converted from four bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A single-precision floating point number formed by four bytes of a byte array.</returns>
    public static float ToSingle(byte[] value) => ToSingle(value, 0);

    /// <summary>
    /// Returns a double-precision floating point number converted from eight bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A double-precision floating point number formed by eight bytes of a specified span.</returns>
    public static double ToDouble(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(double));
        return ToDoubleCore(value);
    }

    /// <summary>
    /// Returns a double-precision floating point number converted from eight bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A double-precision floating point number formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
    public static double ToDouble(byte[] value, int startIndex) =>
        ToDoubleCore(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(double)));

    /// <summary>
    /// Returns a double-precision floating point number converted from eight bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A double-precision floating point number formed by eight bytes of a byte array.</returns>
    public static double ToDouble(byte[] value) => ToDouble(value, 0);

    /// <summary>
    /// Returns a decimal number converted from sixteen bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A decimal number formed by sixteen of a specified span.</returns>
    public static decimal ToDecimal(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(decimal));
        return ToDecimalCore(value);
    }

    /// <summary>
    /// Returns a decimal number converted from sixteen bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A decimal number formed by sixteen bytes beginning at <paramref name="startIndex"/>.</returns>
    public static decimal ToDecimal(byte[] value, int startIndex) =>
        ToDecimalCore(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(decimal)));

    /// <summary>
    /// Returns a decimal number converted from sixteen bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A decimal number formed by sixteen of a byte array.</returns>
    public static decimal ToDecimal(byte[] value) => ToDecimal(value, 0);

    /// <summary>
    /// Returns a <see cref="Boolean"/> value converted from the first byte of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns><c>true</c> if the first byte of a specified span is nonzero; otherwise, <c>false</c>.</returns>
    public static bool ToBoolean(ReadOnlySpan<byte> value)
    {
        BitConverterServices.ValidateToArguments(value, sizeof(bool));
        return BitConverterServices.ToBooleanCore(value);
    }

    /// <summary>
    /// Returns a <see cref="Boolean"/> value converted from one byte at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns><c>true</c> if the byte at startIndex in value is nonzero; otherwise, <c>false</c>.</returns>
    public static bool ToBoolean(byte[] value, int startIndex) =>
        BitConverterServices.ToBooleanCore(
            BitConverterServices.ValidateToArguments(value, startIndex, sizeof(bool)));

    /// <summary>
    /// Returns a Boolean value converted from the first byte of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns><c>true</c> if the first byte of a byte array is nonzero; otherwise, <c>false</c>.</returns>
    public static bool ToBoolean(byte[] value) => ToBoolean(value, 0);

    #region IBitConverter Implementation

    void IBitConverter.FillBytes(short value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(ushort value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(int value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(uint value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(long value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(ulong value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(float value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(double value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(decimal value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(bool value, byte[] buffer, int startIndex) => FillBytes(value, buffer, startIndex);

    void IBitConverter.FillBytes(short value, byte[] buffer) => FillBytes(value, buffer);

    void IBitConverter.FillBytes(ushort value, byte[] buffer) => FillBytes(value, buffer);

    void IBitConverter.FillBytes(int value, byte[] buffer) => FillBytes(value, buffer);

    void IBitConverter.FillBytes(uint value, byte[] buffer) => FillBytes(value, buffer);

    void IBitConverter.FillBytes(long value, byte[] buffer) => FillBytes(value, buffer);

    void IBitConverter.FillBytes(ulong value, byte[] buffer) => FillBytes(value, buffer);

    void IBitConverter.FillBytes(float value, byte[] buffer) => FillBytes(value, buffer);

    void IBitConverter.FillBytes(double value, byte[] buffer) => FillBytes(value, buffer);

    void IBitConverter.FillBytes(decimal value, byte[] buffer) => FillBytes(value, buffer);

    void IBitConverter.FillBytes(bool value, byte[] buffer) => FillBytes(value, buffer);

    byte[] IBitConverter.GetBytes(short value) => GetBytes(value);

    byte[] IBitConverter.GetBytes(ushort value) => GetBytes(value);

    byte[] IBitConverter.GetBytes(int value) => GetBytes(value);

    byte[] IBitConverter.GetBytes(uint value) => GetBytes(value);

    byte[] IBitConverter.GetBytes(long value) => GetBytes(value);

    byte[] IBitConverter.GetBytes(ulong value) => GetBytes(value);

    byte[] IBitConverter.GetBytes(float value) => GetBytes(value);

    byte[] IBitConverter.GetBytes(double value) => GetBytes(value);

    byte[] IBitConverter.GetBytes(decimal value) => GetBytes(value);

    byte[] IBitConverter.GetBytes(bool value) => GetBytes(value);

    ushort IBitConverter.ToUInt16(ReadOnlySpan<byte> value) => ToUInt16(value);

    short IBitConverter.ToInt16(ReadOnlySpan<byte> value) => ToInt16(value);

    int IBitConverter.ToInt32(ReadOnlySpan<byte> value) => ToInt32(value);

    uint IBitConverter.ToUInt32(ReadOnlySpan<byte> value) => ToUInt32(value);

    long IBitConverter.ToInt64(ReadOnlySpan<byte> value) => ToInt64(value);

    ulong IBitConverter.ToUInt64(ReadOnlySpan<byte> value) => ToUInt64(value);

    float IBitConverter.ToSingle(ReadOnlySpan<byte> value) => ToSingle(value);

    double IBitConverter.ToDouble(ReadOnlySpan<byte> value) => ToDouble(value);

    decimal IBitConverter.ToDecimal(ReadOnlySpan<byte> value) => ToDecimal(value);

    bool IBitConverter.ToBoolean(ReadOnlySpan<byte> value) => ToBoolean(value);

    #region Compatibility

    ushort IBitConverter.ToUInt16(byte[] value, int startIndex) => ToUInt16(value, startIndex);

    short IBitConverter.ToInt16(byte[] value, int startIndex) => ToInt16(value, startIndex);

    int IBitConverter.ToInt32(byte[] value, int startIndex) => ToInt32(value, startIndex);

    uint IBitConverter.ToUInt32(byte[] value, int startIndex) => ToUInt32(value, startIndex);

    long IBitConverter.ToInt64(byte[] value, int startIndex) => ToInt64(value, startIndex);

    ulong IBitConverter.ToUInt64(byte[] value, int startIndex) => ToUInt64(value, startIndex);

    float IBitConverter.ToSingle(byte[] value, int startIndex) => ToSingle(value, startIndex);

    double IBitConverter.ToDouble(byte[] value, int startIndex) => ToDouble(value, startIndex);

    decimal IBitConverter.ToDecimal(byte[] value, int startIndex) => ToDecimal(value, startIndex);

    bool IBitConverter.ToBoolean(byte[] value, int startIndex) => ToBoolean(value, startIndex);

    short IBitConverter.ToInt16(byte[] value) => ToInt16(value);

    int IBitConverter.ToInt32(byte[] value) => ToInt32(value);

    long IBitConverter.ToInt64(byte[] value) => ToInt64(value);

    ushort IBitConverter.ToUInt16(byte[] value) => ToUInt16(value);

    uint IBitConverter.ToUInt32(byte[] value) => ToUInt32(value);

    ulong IBitConverter.ToUInt64(byte[] value) => ToUInt64(value);

    float IBitConverter.ToSingle(byte[] value) => ToSingle(value);

    double IBitConverter.ToDouble(byte[] value) => ToDouble(value);

    decimal IBitConverter.ToDecimal(byte[] value) => ToDecimal(value);

    bool IBitConverter.ToBoolean(byte[] value) => ToBoolean(value);

    #endregion

    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static volatile IBitConverter? m_Instance;

    /// <summary>
    /// Returns a default bit converter instance for little-endian byte order.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [CLSCompliant(false)]
    public static IBitConverter Instance => m_Instance ??= new LittleEndianBitConverter();
}
