﻿#pragma warning disable CS3010 // CLS-compliant interfaces must have only CLS-compliant members

namespace Gapotchenko.FX;

/// <summary>
/// Defines interface of a byte order converter.
/// </summary>
public interface IBitConverter
{
    /// <summary>
    /// Returns the specified 16-bit signed integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 2.</returns>
    byte[] GetBytes(short value);

    /// <summary>
    /// Returns the specified 16-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 2.</returns>
    [CLSCompliant(false)]
    byte[] GetBytes(ushort value);

    /// <summary>
    /// Returns the specified 32-bit signed integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    byte[] GetBytes(int value);

    /// <summary>
    /// Returns the specified 32-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    [CLSCompliant(false)]
    byte[] GetBytes(uint value);

    /// <summary>
    /// Returns the specified 64-bit signed integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    byte[] GetBytes(long value);

    /// <summary>
    /// Returns the specified 64-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    [CLSCompliant(false)]
    byte[] GetBytes(ulong value);

    /// <summary>
    /// Returns the specified single-precision floating point value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    byte[] GetBytes(float value);

    /// <summary>
    /// Returns the specified double-precision floating point value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    byte[] GetBytes(double value);

    /// <summary>
    /// Returns the specified decimal value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 16.</returns>
    byte[] GetBytes(decimal value);

    /// <summary>
    /// Returns the specified <see cref="bool"/> value as an array of bytes.
    /// </summary>
    /// <param name="value">A <see cref="bool"/> value.</param>
    /// <returns>An array of bytes with length 1.</returns>
    byte[] GetBytes(bool value);

    /// <summary>
    /// Fills the array with two bytes of the specified 16-bit signed integer value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    void FillBytes(short value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with two bytes of the specified 16-bit unsigned integer value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    [CLSCompliant(false)]
    void FillBytes(ushort value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with four bytes of the specified 32-bit signed integer value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is be stored.</param>
    void FillBytes(int value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with four bytes of the specified 32-bit unsigned integer value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    [CLSCompliant(false)]
    void FillBytes(uint value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with eight bytes of the specified 64-bit signed integer value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    void FillBytes(long value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with eight bytes of the specified 64-bit unsigned integer value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    [CLSCompliant(false)]
    void FillBytes(ulong value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with four bytes of the specified single-precision floating point value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    void FillBytes(float value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with eight bytes of the specified double-precision floating point value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    void FillBytes(double value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with sixteen bytes of the specified decimal value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    void FillBytes(decimal value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with one byte of the specified <see cref="bool"/> value beginning at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="value">A <see cref="bool"/> value.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    /// <param name="startIndex">The start index where converted value is to be stored at <paramref name="buffer"/>.</param>
    void FillBytes(bool value, byte[] buffer, int startIndex);

    /// <summary>
    /// Fills the array with two bytes of the specified 16-bit signed integer value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    void FillBytes(short value, byte[] buffer);

    /// <summary>
    /// Fills the array with two bytes of the specified 16-bit unsigned integer value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    [CLSCompliant(false)]
    void FillBytes(ushort value, byte[] buffer);

    /// <summary>
    /// Fills the array with four bytes of the specified 32-bit signed integer value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    void FillBytes(int value, byte[] buffer);

    /// <summary>
    /// Fills the array with four bytes of the specified 32-bit unsigned integer value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    [CLSCompliant(false)]
    void FillBytes(uint value, byte[] buffer);

    /// <summary>
    /// Fills the array with eight bytes of the specified 64-bit signed integer value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    void FillBytes(long value, byte[] buffer);

    /// <summary>
    /// Fills the array with eight bytes of the specified 64-bit unsigned integer value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    [CLSCompliant(false)]
    void FillBytes(ulong value, byte[] buffer);

    /// <summary>
    /// Fills the array with four bytes of the specified single-precision floating point value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    void FillBytes(float value, byte[] buffer);

    /// <summary>
    /// Fills the array with eight bytes of the specified double-precision floating point value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    void FillBytes(double value, byte[] buffer);

    /// <summary>
    /// Fills the array with sixteen bytes of the specified decimal value.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    void FillBytes(decimal value, byte[] buffer);

    /// <summary>
    /// Fills the array with one byte of the specified <see cref="bool"/> value beginning.
    /// </summary>
    /// <param name="value">A <see cref="bool"/> value.</param>
    /// <param name="buffer">The array of bytes to store converted value at.</param>
    void FillBytes(bool value, byte[] buffer);

    /// <summary>
    /// Returns a 16-bit signed integer converted from the first two bytes of a specified span.
    /// </summary>
    /// <param name="value">The byte span.</param>
    /// <returns>A 16-bit signed integer formed by the first two bytes of a specified span.</returns>
    short ToInt16(ReadOnlySpan<byte> value);

    /// <summary>
    /// Returns a 16-bit unsigned integer converted from the first two bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 16-bit unsigned integer formed by the first two bytes of a specified span.</returns>
    [CLSCompliant(false)]
    ushort ToUInt16(ReadOnlySpan<byte> value);

    /// <summary>
    /// Returns a 32-bit signed integer converted from the first four bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 32-bit signed integer formed by the first four bytes of a specified span.</returns>
    int ToInt32(ReadOnlySpan<byte> value);

    /// <summary>
    /// Returns a 32-bit unsigned integer converted from the first four bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 32-bit unsigned integer formed by the first four bytes of a specified span.</returns>
    [CLSCompliant(false)]
    uint ToUInt32(ReadOnlySpan<byte> value);

    /// <summary>
    /// Returns a 64-bit signed integer converted from the first eight bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 64-bit signed integer formed by the first eight bytes of a specified span.</returns>
    long ToInt64(ReadOnlySpan<byte> value);

    /// <summary>
    /// Returns a 64-bit unsigned integer converted from the first eight bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 64-bit unsigned integer formed by the first eight bytes of a specified span.</returns>
    [CLSCompliant(false)]
    ulong ToUInt64(ReadOnlySpan<byte> value);

    /// <summary>
    /// Returns a single-precision floating point number converted from four bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A single-precision floating point number formed by four bytes of a specified span.</returns>
    float ToSingle(ReadOnlySpan<byte> value);

    /// <summary>
    /// Returns a double-precision floating point number converted from eight bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A double-precision floating point number formed by eight bytes of a specified span.</returns>
    double ToDouble(ReadOnlySpan<byte> value);

    /// <summary>
    /// Returns a decimal number converted from sixteen bytes of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A decimal number formed by sixteen of a specified span.</returns>
    decimal ToDecimal(ReadOnlySpan<byte> value);

    /// <summary>
    /// Returns a <see cref="Boolean"/> value converted from the first byte of a specified span.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns><see langword="true"/> if the first byte of a specified span is nonzero; otherwise, <see langword="false"/>.</returns>
    bool ToBoolean(ReadOnlySpan<byte> value);

    #region Compatibility

    /// <summary>
    /// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 16-bit signed integer formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
    short ToInt16(byte[] value, int startIndex);

    /// <summary>
    /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 16-bit unsigned integer formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
    [CLSCompliant(false)]
    ushort ToUInt16(byte[] value, int startIndex);

    /// <summary>
    /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 32-bit signed integer formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
    int ToInt32(byte[] value, int startIndex);

    /// <summary>
    /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 32-bit unsigned integer formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
    [CLSCompliant(false)]
    uint ToUInt32(byte[] value, int startIndex);

    /// <summary>
    /// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 64-bit signed integer formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
    long ToInt64(byte[] value, int startIndex);

    /// <summary>
    /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 64-bit unsigned integer formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
    [CLSCompliant(false)]
    ulong ToUInt64(byte[] value, int startIndex);

    /// <summary>
    /// Returns a single-precision floating point number converted from four bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A single-precision floating point number formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
    float ToSingle(byte[] value, int startIndex);

    /// <summary>
    /// Returns a double-precision floating point number converted from eight bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A double-precision floating point number formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
    double ToDouble(byte[] value, int startIndex);

    /// <summary>
    /// Returns a decimal number converted from sixteen bytes at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A decimal number formed by sixteen bytes beginning at <paramref name="startIndex"/>.</returns>
    decimal ToDecimal(byte[] value, int startIndex);

    /// <summary>
    /// Returns a <see cref="Boolean"/> value converted from one byte at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns><see langword="true"/> if the byte at startIndex in value is nonzero; otherwise, <see langword="false"/>.</returns>
    bool ToBoolean(byte[] value, int startIndex);

    /// <summary>
    /// Returns a 16-bit signed integer converted from the first two bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 16-bit signed integer formed by the first two bytes of a byte array.</returns>
    short ToInt16(byte[] value);

    /// <summary>
    /// Returns a 16-bit unsigned integer converted from the first two bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 16-bit unsigned integer formed by the first two bytes of a byte array.</returns>
    [CLSCompliant(false)]
    ushort ToUInt16(byte[] value);

    /// <summary>
    /// Returns a 32-bit signed integer converted from the first four bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 32-bit signed integer formed by the first four bytes of a byte array.</returns>
    int ToInt32(byte[] value);

    /// <summary>
    /// Returns a 32-bit unsigned integer converted from the first four bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 32-bit unsigned integer formed by the first four bytes of a byte array.</returns>
    [CLSCompliant(false)]
    uint ToUInt32(byte[] value);

    /// <summary>
    /// Returns a 64-bit signed integer converted from the first eight bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 64-bit signed integer formed by the first eight bytes of a byte array.</returns>
    long ToInt64(byte[] value);

    /// <summary>
    /// Returns a 64-bit unsigned integer converted from the first eight bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A 64-bit unsigned integer formed by the first eight bytes of a byte array.</returns>
    [CLSCompliant(false)]
    ulong ToUInt64(byte[] value);

    /// <summary>
    /// Returns a single-precision floating point number converted from four bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A single-precision floating point number formed by four bytes of a byte array.</returns>
    float ToSingle(byte[] value);

    /// <summary>
    /// Returns a double-precision floating point number converted from eight bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A double-precision floating point number formed by eight bytes of a byte array.</returns>
    double ToDouble(byte[] value);

    /// <summary>
    /// Returns a decimal number converted from sixteen bytes of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns>A decimal number formed by sixteen of a byte array.</returns>
    decimal ToDecimal(byte[] value);

    /// <summary>
    /// Returns a <see cref="Boolean"/> value converted from the first byte of a byte array.
    /// </summary>
    /// <param name="value">An array of bytes.</param>
    /// <returns><see langword="true"/> if the first byte of a byte array is nonzero; otherwise, <see langword="false"/>.</returns>
    bool ToBoolean(byte[] value);

    #endregion
}
