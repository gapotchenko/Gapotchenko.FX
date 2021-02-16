using System;

namespace Gapotchenko.FX.Math
{
    [Flags]
    enum IntervalFlags : byte
    {
        None = 0,
        InclusiveLowerBound = 1 << 0,
        InclusiveUpperBound = 1 << 1,
        HasLowerBound = 1 << 2,
        HasUpperBound = 1 << 3
    }
}
