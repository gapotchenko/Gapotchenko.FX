using Gapotchenko.FX.Runtime.InteropServices;
using System.Reflection;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal;

/// <summary>
/// Intrinsic patcher base.
/// </summary>
abstract unsafe class Patcher
{
    public enum PatchResult
    {
        Success,
        UnexpectedEpilogue,
        InvalidAlignment
    }

    public abstract PatchResult PatchMethod(MethodInfo method, byte[] code);

    protected static bool IsSupportedPrologue(byte[][] supportedPrologues, byte* buffer)
    {
        foreach (byte[] prologue in supportedPrologues)
        {
            bool match = true;

            for (int i = 0; i < prologue.Length; i++)
            {
                if (buffer[i] != prologue[i])
                {
                    match = false;
                    break;
                }
            }

            if (match)
                return true;
        }

        return false;
    }

    protected static byte* Write(byte* dest, params byte[] data)
    {
        int size = data.Length;
        fixed (byte* src = data)
            MemoryOperations.BlockCopy(src, dest, size);
        return dest + size;
    }
}
