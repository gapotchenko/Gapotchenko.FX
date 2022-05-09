using Gapotchenko.FX.Runtime.InteropServices;
using System.Reflection;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal
{
    /// <summary>
    /// Intrinsic patcher base.
    /// </summary>
    abstract unsafe class Patcher
    {
        public enum PatchResult
        {
            Success,
            UnexpectedEpilogue
        }

        public abstract PatchResult PatchMethod(MethodInfo method, byte[] code);

        protected static byte* Write(byte* dest, params byte[] data)
        {
            int size = data.Length;
            fixed (byte* src = data)
                MemoryOperations.BlockCopy(src, dest, size);
            return dest + size;
        }
    }
}
