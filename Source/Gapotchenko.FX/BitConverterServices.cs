using Gapotchenko.FX.Properties;
using System;

namespace Gapotchenko.FX;

static class BitConverterServices
{
    public static void ValidateToArguments(byte[] value, int startIndex, int size)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        ValidateArguments(value, startIndex, size);
    }

    public static void ValidateFillArguments(byte[] buffer, int startIndex, int size)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        ValidateArguments(buffer, startIndex, size);
    }

    static void ValidateArguments(byte[] array, int startIndex, int size)
    {
        if (startIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex), Resources.ArgumentOutOfRange_Index);
        if (startIndex > array.Length - size)
            throw new ArgumentException(Resources.Argument_IndexedArrayTooSmall);
    }

    public static void FillBytes(bool value, byte[] buffer, int startIndex)
    {
        ValidateFillArguments(buffer, startIndex, 1);

        buffer[startIndex] = value ? (byte)1 : (byte)0;
    }

    public static bool ToBoolean(byte[] value, int startIndex)
    {
        ValidateToArguments(value, startIndex, 1);

        return value[startIndex] != 0;
    }
}
