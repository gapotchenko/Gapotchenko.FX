using Gapotchenko.FX.Memory.Properties;
using System.Diagnostics;

namespace Gapotchenko.FX.Memory;

static class StreamHelpers
{
    public static void ValidateBuffer(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), Resources.ArgumentOutOfRangeException_NonNegativeNumberRequired);
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), Resources.ArgumentOutOfRangeException_NonNegativeNumberRequired);
        if (buffer.Length - offset < count)
        {
            throw new ArgumentException(
                "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.",
                nameof(count));
        }
    }

    public static void ValidatePosition(long value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), Resources.ArgumentOutOfRangeException_NonNegativeNumberRequired);
    }

    public static int SetPosition(long value)
    {
        if (value > int.MaxValue)
            ThrowLengthMustBeNonNegativeAndLessThan2GB(nameof(value));

        return (int)value;
    }

    public static int Seek(long offset, SeekOrigin origin, int position, int length)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                if (offset < 0)
                    ThrowSeekBeforeBegin();
                return (int)offset;

            case SeekOrigin.Current:
                return SeekRelativeTo(position, offset);

            case SeekOrigin.End:
                return SeekRelativeTo(length, offset);

            default:
                throw new ArgumentOutOfRangeException(nameof(origin));
        }
    }

    static int SeekRelativeTo(int @base, long offset)
    {
        var newPosition = @base + (int)offset;
        if (newPosition < 0 || @base + offset < 0 /* checking for overflow */)
            ThrowSeekBeforeBegin();
        return newPosition;
    }

    [DoesNotReturn, StackTraceHidden]
    public static void ThrowNotWritable() =>
        throw new NotSupportedException("Stream does not support writing.");

    [DoesNotReturn, StackTraceHidden]
    public static void ThrowNotPubliclyVisible() =>
        throw new UnauthorizedAccessException("Stream memory buffer cannot be accessed.");

    [DoesNotReturn, StackTraceHidden]
    public static void ThrowNotExpandable() =>
        throw new NotSupportedException("Stream does not support expansion.");

    [DoesNotReturn, StackTraceHidden]
    public static void ThrowSeekBeforeBegin() =>
        throw new IOException("An attempt was made to move the position before the beginning of the stream.");

    [DoesNotReturn, StackTraceHidden]
    public static void ThrowTooLong() =>
        throw new IOException("Stream was too long.");

    [DoesNotReturn, StackTraceHidden]
    public static void ThrowClosedAndDisposed() =>
        throw new ObjectDisposedException(null, "Cannot access a closed stream.");

    [DoesNotReturn, StackTraceHidden]
    public static void ThrowLengthMustBeNonNegativeAndLessThan2GB(string paramName) =>
        throw new ArgumentOutOfRangeException(
            paramName,
            string.Format(
                Resources.StreamLengthMustBeNonNegativeAndLessThanX,
                "2^31 - 1"));
}
