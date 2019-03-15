using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX
{
    [StructLayout(LayoutKind.Explicit)]
    struct ReinterpretCastGround64
    {
        [FieldOffset(0)]
        public long Int64;

        [FieldOffset(0)]
        public double Double;
    }
}
