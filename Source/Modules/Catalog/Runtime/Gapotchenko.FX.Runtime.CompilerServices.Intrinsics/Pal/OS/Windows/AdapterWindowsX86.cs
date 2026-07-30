// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Intrinsic adapter for Windows OS and Intel-based 32-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class AdapterWindowsX86 : AdapterX86
{
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        var instructions = GetMethodInstructions(method);

        int patchablePrologueSize = Util.GetPatchablePrologueSize(instructions, m_SupportedPrologues);
        if (patchablePrologueSize < 0)
            return PatchResult.UnexpectedPrologue;

        instructions = instructions[..Math.Min(patchablePrologueSize, instructions.Length)];

        PatchResult result;
#if TFF_CER
        // Ensure that code changes are atomic by using the constrained execution region.
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
#endif
        {
            result = ApplyPatch(instructions, code);
        }
        return result;
    }

    // The table defines a conservative minimum method body extent:
    // complete prologue plus the shortest matching restoration/return sequence.
    static readonly (byte[] Instructions, int Size)[] m_SupportedPrologues =
    [
        ([0x55, 0x8b, 0xec], 3 + 2),             // PUSH EBP; MOV EBP,ESP
        ([0x8b, 0xff, 0x55, 0x8b, 0xec], 5 + 2), // MOV EDI,EDI; PUSH EBP; MOV EBP,ESP
        ([0x53, 0x56, 0x57], 3 + 4),             // PUSH EBX; PUSH ESI; PUSH EDI
        ([0x53, 0x83, 0xec], 4 + 5),             // PUSH EBX; SUB ESP,imm8
        ([0x56, 0x83, 0xec], 4 + 5),             // PUSH ESI; SUB ESP,imm8
        ([0x57, 0x83, 0xec], 4 + 5),             // PUSH EDI; SUB ESP,imm8
        ([0x83, 0xec], 3 + 4),                   // SUB ESP,imm8
        ([0x81, 0xec], 6 + 7),                   // SUB ESP,imm32
    ];

    static unsafe Span<byte> GetMethodInstructions(MethodInfo method)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        byte* p = (byte*)method.MethodHandle.GetFunctionPointer();
        p = SkipBranches(p);

        // It is impossible to determine the exact method instruction boundaries for x86 architecture.
        // Instead, use memory region information as a coarse approximation.
        if (NativeMethods.VirtualQuery(p, out var memoryInfo, (nuint)sizeof(NativeMethods.MemoryBasicInformation)) == 0 ||
            memoryInfo.State != NativeMethods.PageState.MemCommit ||
            (memoryInfo.Protect & (NativeMethods.PageProtect.NoAccess | NativeMethods.PageProtect.Guard)) != 0)
        {
            return [];
        }

        byte* regionStart = (byte*)memoryInfo.BaseAddress;
        byte* regionEnd = regionStart + memoryInfo.RegionSize;
        if (p < regionStart || p >= regionEnd)
            return [];

        nuint regionLength = (nuint)(regionEnd - p);
        if (regionLength > int.MaxValue)
            return [];

        return new(p, (int)regionLength);
    }

    static PatchResult ApplyPatch(Span<byte> instructions, ReadOnlySpan<byte> code)
    {
        int patchSize = code.Length + 1 /* RET */;

        var trampoline = Span<byte>.Empty;
        if (patchSize > instructions.Length)
        {
            if (instructions.Length < JmpRel32Size)
                return PatchResult.NoSpace;

            trampoline = TrampolineAllocator.Allocate(patchSize);

            using var trampolineScope = VirtualProtectionScope.Create(trampoline, NativeMethods.PageProtect.ExecuteReadWrite);
            code.CopyTo(trampoline);
            trampoline[code.Length] = Ret;
            trampolineScope.FlushInstructions();
        }

        // Temporarily allow memory modification in order to apply the intrinsic code.
        using var scope = VirtualProtectionScope.Create(instructions, NativeMethods.PageProtect.ExecuteReadWrite);

        if (!trampoline.IsEmpty)
        {
            // Redirect the method to the trampoline.
            nint offset = Unsafe.ByteOffset(
                ref MemoryMarshal.GetReference(instructions),
                ref MemoryMarshal.GetReference(trampoline));
            int displacement = (int)(offset - JmpRel32Size);

            // "JMP rel32" instruction is used because it has the most compact form in x86 instruction set.
            instructions[0] = JmpRel32;
            BinaryPrimitives.WriteInt32LittleEndian(instructions[1..], displacement);
        }
        else
        {
            // Put the intrinsic code.
            code.CopyTo(instructions);

            // End the method with a RET instruction.
            instructions[code.Length] = Ret;
        }

        scope.FlushInstructions();

        return PatchResult.Success;
    }
}
