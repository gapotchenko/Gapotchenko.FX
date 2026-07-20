// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

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
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        var instructions = GetMethodInstructions(method);
        if (!IsSupportedPrologue(instructions))
            return PatchResult.UnexpectedPrologue;

        int patchSize = code.Length + 1 /* RET */;
        if (patchSize > instructions.Length)
            return PatchResult.NoSpace;

        instructions = instructions[..patchSize];

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
            using var scope = VirtualProtectionScope.Create(instructions, NativeMethods.PageProtect.ExecuteReadWrite);

            // Put the intrinsic code.
            code.CopyTo(instructions);

            // End the method with a RET instruction.
            instructions[code.Length] = 0xc3;

            scope.FlushInstructions();
        }

        return PatchResult.Success;
    }

    static bool IsSupportedPrologue(ReadOnlySpan<byte> instructions)
    {
        foreach (byte[] prologue in m_SupportedPrologues)
        {
            if (instructions.StartsWith(prologue))
                return true;
        }
        return false;
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

    static Span<byte> GetMethodInstructions(MethodInfo method)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        byte* p = (byte*)method.MethodHandle.GetFunctionPointer();
        p = SkipBranches(p);

        return new(p, int.MaxValue);

        static byte* SkipBranches(byte* p)
        {
            while (*p == 0xe9)
            {
                int displacement = *(int*)(p + 1) + 5;
                p += displacement;
            }
            return p;
        }
    }
}
