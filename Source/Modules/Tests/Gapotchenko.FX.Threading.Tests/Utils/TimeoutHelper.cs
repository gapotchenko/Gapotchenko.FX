namespace Gapotchenko.FX.Threading.Tests.Utils;

static class TimeoutHelper
{
    public static int GetMillisecondsTimeout(TimeSpan timeout) => checked((int)timeout.TotalMilliseconds);
}
