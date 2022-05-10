using System;

namespace Gapotchenko.FX.Math
{
    [Flags]
    enum IntervalFlags : byte
    {
        None = 0,
        LeftClosed = 1 << 0,
        RightClosed = 1 << 1,
        LeftBounded = 1 << 2,
        RightBounded = 1 << 3
    }
}
