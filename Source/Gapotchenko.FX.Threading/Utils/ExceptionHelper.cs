using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Threading.Utils;

static class ExceptionHelper
{
    public static void ThrowIfArgumentIsNull(object? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
    }

    public static void ValidateTimeoutArgument(TimeSpan value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        long milliseconds = (long)value.TotalMilliseconds;
        if (milliseconds < -1 || milliseconds > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "The value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0, or a positive integer less than or equal to the maximum allowed timer duration.");
        }
    }
}
