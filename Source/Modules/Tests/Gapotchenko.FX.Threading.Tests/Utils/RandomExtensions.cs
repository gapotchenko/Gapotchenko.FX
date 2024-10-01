namespace Gapotchenko.FX.Threading.Tests.Utils;

static class RandomExtensions
{
    public static bool NextBoolean(this Random random) => random.Next(2) == 0;
}
