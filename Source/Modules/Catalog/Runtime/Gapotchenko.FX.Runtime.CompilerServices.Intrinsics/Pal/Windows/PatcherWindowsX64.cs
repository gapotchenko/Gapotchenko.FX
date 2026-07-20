using System.Reflection;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Windows;

/// <summary>
/// Intrinsic patcher for Windows OS and AMD-based 64-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed unsafe class PatcherWindowsX64 : Patcher
{
    public override PatchResult PatchMethod(MethodInfo method, byte[] code)
    {
        byte* p = GetPointerToMethodInstructions(method);
        if (!IsSupportedPrologue(m_SupportedPrologues, p))
            return PatchResult.UnexpectedEpilogue;

        int codeSize = code.Length;

#if TFF_CER
        // Ensure that code changes are atomic by using the constrained execution region.
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
#endif
        {
            // Temporarily allow memory modification in order to apply the intrinsic code.
            using var scope = new VirtualProtectionScope(p, codeSize + 1, NativeMethods.PageProtect.ExecuteReadWrite);

            // Put the intrinsic code.
            p = Write(p, code);

            // End method with a 'ret' instruction.
            Write(p, 0xc3);
        }

        return PatchResult.Success;
    }

    static readonly byte[][] m_SupportedPrologues =
    [
        [0x48, 0x83, 0xec, 0x18, 0x48, 0x89, 0x34, 0x24], // Mono 5.18.1, x64
        [0x48, 0x83, 0xec, 0x28],
        [0x48, 0x89, 0x54, 0x24],
        [0x53, 0x48, 0x83, 0xec, 0x20],
        [0x55, 0x48, 0x83, 0xec, 0x20],
        [0x55, 0x57, 0x56, 0x48, 0x83, 0xec, 0x30],
        [0x56, 0x48, 0x83, 0xec, 0x20],
        [0x57, 0x56, 0x48, 0x83, 0xec, 0x28], // Windows 10 x64, NGen 4.7.2
    ];

    static byte* GetPointerToMethodInstructions(MethodInfo method)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        byte* p = (byte*)method.MethodHandle.GetFunctionPointer();
        p = SkipJumps(p);
        return p;

        static byte* SkipJumps(byte* p)
        {
            while (*p == 0xe9)
            {
                int delta = *(int*)(p + 1) + 5;
                p += delta;
            }
            return p;
        }
    }
}
