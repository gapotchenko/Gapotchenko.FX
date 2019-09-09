using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX
{
    static class BitConverterEx
    {
#if TFF_BITCONVERTER_SINGLETOINT32BITS
        public static int SingleToInt32Bits(float value) => BitConverter.SingleToInt32Bits(value);

        public static float Int32BitsToSingle(int value) => BitConverter.Int32BitsToSingle(value);
#else
        [StructLayout(LayoutKind.Explicit)]
        struct ReinterpretCastGround32
        {
            [FieldOffset(0)]
            public int Int32;

            [FieldOffset(0)]
            public float Single;
        }

        public static int SingleToInt32Bits(float value)
        {
            var rcg = new ReinterpretCastGround32();
            rcg.Single = value;
            return rcg.Int32;
        }

        public static float Int32BitsToSingle(int value)
        {
            var rcg = new ReinterpretCastGround32();
            rcg.Int32 = value;
            return rcg.Single;
        }
#endif
    }
}
