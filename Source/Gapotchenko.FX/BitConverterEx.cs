using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX
{
    static class BitConverterEx
    {
#if !TFF_BITCONVERTER_SINGLETOINT32BITS
        [StructLayout(LayoutKind.Explicit)]
        struct ReinterpretCastGround32
        {
            [FieldOffset(0)]
            public int Int32;

            [FieldOffset(0)]
            public float Single;
        }
#endif

        public static int SingleToInt32Bits(float value)
        {
#if TFF_BITCONVERTER_SINGLETOINT32BITS
            return BitConverter.SingleToInt32Bits(value);
#else
            var rcg = new ReinterpretCastGround32();
            rcg.Single = value;
            return rcg.Int32;
#endif
        }

        public static float Int32BitsToSingle(int value)
        {
#if TFF_BITCONVERTER_SINGLETOINT32BITS
            return BitConverter.Int32BitsToSingle(value);
#else
            var rcg = new ReinterpretCastGround32();
            rcg.Int32 = value;
            return rcg.Single;
#endif
        }
    }
}
