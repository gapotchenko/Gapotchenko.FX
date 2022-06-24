using System;


/* Unmerged change from project 'Gapotchenko.FX.Reflection.Loader (net5.0)'
Before:
namespace Gapotchenko.FX.Reflection.Loader.Polyfills
{
#if !TFF_STRING_OPWITH_CHAR
After:
namespace Gapotchenko.FX.Reflection.Loader.Polyfills;
#if !TFF_STRING_OPWITH_CHAR
*/

/* Unmerged change from project 'Gapotchenko.FX.Reflection.Loader (netcoreapp3.0)'
Before:
namespace Gapotchenko.FX.Reflection.Loader.Polyfills
{
#if !TFF_STRING_OPWITH_CHAR
After:
namespace Gapotchenko.FX.Reflection.Loader.Polyfills;
#if !TFF_STRING_OPWITH_CHAR
*/

/* Unmerged change from project 'Gapotchenko.FX.Reflection.Loader (netcoreapp2.0)'
Before:
namespace Gapotchenko.FX.Reflection.Loader.Polyfills
{
#if !TFF_STRING_OPWITH_CHAR
After:
namespace Gapotchenko.FX.Reflection.Loader.Polyfills;
#if !TFF_STRING_OPWITH_CHAR
*/
namespace Gapotchenko.FX.Reflection.Loader.Polyfills;

#if !TFF_STRING_OPWITH_CHAR
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
