using System;

namespace Gapotchenko.FX.Diagnostics.Implementation
{
    /// <summary>
    /// Universal pointer.
    /// </summary>
    readonly struct UniPtr
    {
        public UniPtr(IntPtr p)
        {
            Value = p.ToInt64();
            Size = IntPtr.Size;
        }

        public UniPtr(long p)
        {
            Value = p;
            Size = sizeof(long);
        }

        public readonly long Value;
        public readonly int Size;

        public static implicit operator IntPtr(UniPtr p) => new IntPtr(p.Value);

        public static implicit operator UniPtr(IntPtr p) => new UniPtr(p);

        public override readonly string ToString() => Value.ToString();

        public readonly bool FitsInNativePointer => Size <= IntPtr.Size;

        public readonly bool CanBeRepresentedByNativePointer
        {
            get
            {
                int actualSize = Size;

                if (actualSize == 8)
                {
                    if (Value >> 32 == 0)
                        actualSize = 4;
                }

                return actualSize <= IntPtr.Size;
            }
        }

        public readonly long ToInt64() => Value;
    }
}
