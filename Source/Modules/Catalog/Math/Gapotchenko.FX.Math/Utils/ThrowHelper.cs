using System.Diagnostics;

namespace Gapotchenko.FX.Math.Utils;

[StackTraceHidden]
static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowMinCannotBeGreaterThanMaxException<T>(T min, T max) =>
        throw new ArgumentException(string.Format(Properties.Resources.MinCannotBeGreaterThanMax, min, max));
}
