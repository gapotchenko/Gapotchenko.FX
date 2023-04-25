using System.Collections.Specialized;

namespace Gapotchenko.FX.Math.Topology.Utils;

static class BitVector32Extensions
{
    public static bool? GetNullableBooleanValue(this BitVector32 flags, int hasValueFlag, int valueFlag)
    {
        if (flags[hasValueFlag])
            return flags[valueFlag];
        else
            return null;
    }

    public static void SetNullableBooleanValue(ref this BitVector32 flags, int hasValueFlag, int valueFlag, bool? value)
    {
        if (value.HasValue)
        {
            flags[hasValueFlag] = true;
            flags[valueFlag] = value.Value;
        }
        else
        {
            flags[hasValueFlag] = false;
        }
    }
}
