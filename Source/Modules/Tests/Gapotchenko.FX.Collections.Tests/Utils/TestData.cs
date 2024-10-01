namespace Gapotchenko.FX.Collections.Tests.Utils;

static class TestData
{
    public static string CreateString(Random random)
    {
        int length = random.Next(5, 15);
        var bytes = new byte[length];
        random.NextBytes(bytes);

        return Convert.ToBase64String(bytes);
    }

    public static int CreateInt32(Random random) => random.Next();
}
