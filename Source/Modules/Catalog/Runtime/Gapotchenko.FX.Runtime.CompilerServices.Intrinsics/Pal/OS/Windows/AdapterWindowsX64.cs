// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Intrinsic adapter for Windows OS and AMD-based 64-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class AdapterWindowsX64 : AdapterX64
{
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        var methodInstructions = GetMethodInstructions(method);
        if (!IsSupportedPrologue(methodInstructions))
            return PatchResult.UnexpectedPrologue;

        var patchInstructions = code;

        int patchSize = patchInstructions.Length + 1 /* RET */;
        if (patchSize > methodInstructions.Length)
            return PatchResult.NoSpace;

        methodInstructions = methodInstructions[..patchSize];

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
            using var scope = VirtualProtectionScope.Create(methodInstructions, NativeMethods.PageProtect.ExecuteReadWrite);

            // Put the intrinsic code.
            patchInstructions.CopyTo(methodInstructions);

            // End the method with a RET instruction.
            methodInstructions[code.Length] = 0xc3;

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

    static unsafe Span<byte> GetMethodInstructions(MethodInfo method)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        byte* p = (byte*)method.MethodHandle.GetFunctionPointer();
        p = SkipBranches(p);

        // Get instruction boundaries.
        var runtimeFunction = (NativeMethods.RuntimeFunctionX64*)NativeMethods.RtlLookupFunctionEntry(p, out void* imageBase, null);
        if (runtimeFunction == null)
            return [];

        byte* functionStart = (byte*)imageBase + runtimeFunction->BeginAddress;
        byte* functionEnd = (byte*)imageBase + runtimeFunction->EndAddress;
        if (p < functionStart || p >= functionEnd)
            return [];

        nuint length = (nuint)(functionEnd - p);
        if (length > int.MaxValue)
            return [];

        return new(p, (int)length);

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
