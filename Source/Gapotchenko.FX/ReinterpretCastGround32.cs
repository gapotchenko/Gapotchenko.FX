using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX
{
    [StructLayout(LayoutKind.Explicit)]
    struct ReinterpretCastGround32
    {
        [FieldOffset(0)]
        public int Int32;

        [FieldOffset(0)]
        public float Single;
    }
}
