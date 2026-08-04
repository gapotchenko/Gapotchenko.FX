namespace Gapotchenko.FX.Security.Cryptography.Utils;

static class RngUtil
{
    public static void FillNonZero(Span<byte> data)
    {
        RandomNumberGenerator.Fill(data);
        for (int i = 0; i < data.Length; ++i)
        {
            while (data[i] == 0)
                RandomNumberGenerator.Fill(data.Slice(i, 1));
        }
    }
}
