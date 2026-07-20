// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Windows;

/// <summary>
/// Intrinsic patcher for Windows OS and Intel-based 32-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class PatcherWindowsX86 : Patcher
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
        [0x55, 0x8b, 0xec],             // PUSH EBP; MOV EBP,ESP
        [0x8b, 0xff, 0x55, 0x8b, 0xec], // MOV EDI,EDI; PUSH EBP; MOV EBP,ESP
        [0x53, 0x56, 0x57],             // PUSH EBX; PUSH ESI; PUSH EDI
        [0x53, 0x83, 0xec],             // PUSH EBX; SUB ESP,imm8
        [0x56, 0x83, 0xec],             // PUSH ESI; SUB ESP,imm8
        [0x57, 0x83, 0xec],             // PUSH EDI; SUB ESP,imm8
        [0x83, 0xec],                   // SUB ESP,imm8
        [0x81, 0xec],                   // SUB ESP,imm32
    ];

    static unsafe Span<byte> GetMethodInstructions(MethodInfo method)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        byte* p = (byte*)method.MethodHandle.GetFunctionPointer();
        p = SkipBranches(p);

        return new(p, int.MaxValue);

        static byte* SkipBranches(byte* p)
        {
            for (; ; )
            {
                switch (*p)
                {
                    case 0xe9: // JMP rel32
                        p += *(int*)(p + 1) + 5;
                        break;

                    case 0xeb: // JMP rel8
                        p += *(sbyte*)(p + 1) + 2;
                        break;

                    default:
                        return p;
                }
            }
        }
    }
}
