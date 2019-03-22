using Gapotchenko.FX.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    static class BitConverterServices
    {
        public static void ValidateToArguments(byte[] value, int startIndex, int size)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            _ValidateArguments(value, startIndex, size);
        }

        public static void ValidateFillArguments(byte[] buffer, int startIndex, int size)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            _ValidateArguments(buffer, startIndex, size);
        }

        static void _ValidateArguments(byte[] array, int startIndex, int size)
        {
            if (startIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), Resources.ArgumentOutOfRange_Index);
            if (startIndex > array.Length - size)
                throw new ArgumentException(Resources.Argument_IndexedArrayTooSmall);
        }
    }
}
