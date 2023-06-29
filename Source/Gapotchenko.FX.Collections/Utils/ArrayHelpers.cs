namespace Gapotchenko.FX.Collections.Utils;

static class ArrayHelpers
{
    public static int ArrayMaxLength =>
#if NET6_0_OR_GREATER
        Array.MaxLength;
#else
        0x7fffffc7; // the value is hardcoded in .NET BCL
#endif
}
