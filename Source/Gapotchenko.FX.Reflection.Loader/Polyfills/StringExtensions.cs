using System;

namespace Gapotchenko.FX.Reflection.Loader.Polyfills
{
#if !NETCOREAPP
    static class StringExtensions
    {
        public static bool EndsWith(this string s, char value)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            int n = s.Length;
            return n != 0 && s[n - 1] == value;
        }
    }
#endif
}
