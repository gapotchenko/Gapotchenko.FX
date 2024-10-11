namespace Gapotchenko.FX.Reflection.Loader.Polyfills;

#if !TFF_STRING_OPWITH_CHAR

static class StringPolyfills
{
    public static bool EndsWith(this string s, char value)
    {
        int n = s.Length;
        return n != 0 && s[n - 1] == value;
    }
}

#endif
