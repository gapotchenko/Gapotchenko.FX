using Gapotchenko.FX.Properties;

namespace Gapotchenko.FX;

static class BitConverterServices
{
    public static void ValidateToArguments(in ReadOnlySpan<byte> span, int size) =>
        ValidateArguments(span, size);

    static void ValidateArguments(in ReadOnlySpan<byte> span, int size)
    {
        if (size > span.Length)
            throw new ArgumentException(Resources.Argument_SpanTooSmall);
    }

    public static ReadOnlySpan<byte> ValidateToArguments(byte[] value, int startIndex, int size)
    {
        ArgumentNullException.ThrowIfNull(value);
        ValidateArguments(value, startIndex, size);
        return value.AsSpan(startIndex);
    }

    public static void ValidateFillArguments(byte[] buffer, int startIndex, int size)
    {
        ArgumentNullException.ThrowIfNull(buffer);
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

    public static bool ToBooleanCore(ReadOnlySpan<byte> value) => value[0] != 0;
}
