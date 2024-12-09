namespace Gapotchenko.FX.Reflection.Loader.Polyfills;

static class ArrayEqualityComparer
{
    public static bool Equals(byte[]? a, byte[]? b)
    {
        if (a == b)
            return true;
        if (a is null || b is null)
            return false;

        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; ++i)
            if (a[i] != b[i])
                return false;

        return true;
    }

    public static int GetHashCode(byte[] obj)
    {
        // FNV-1a hash algorithm.
        uint hash = 2166136261;
        foreach (byte i in obj)
            hash = (hash ^ i) * 16777619;
        return (int)hash;
    }
}
