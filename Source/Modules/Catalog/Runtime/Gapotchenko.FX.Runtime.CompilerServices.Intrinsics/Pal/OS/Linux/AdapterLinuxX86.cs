// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS9191 // The 'ref' modifier for an argument corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
sealed class AdapterLinuxX86 : AdapterX86
{
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        var instructions = GetMethodInstructions(method);
        int patchablePrologueSize = InstructionOperations.GetPatchablePrologueSize(instructions, m_SupportedPrologues);
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
        ([0x55, 0x8b, 0xec], 3 + 2), // PUSH EBP; MOV EBP,ESP
        ([0x55, 0x89, 0xe5], 3 + 2), // PUSH EBP; MOV EBP,ESP
        ([0x53, 0x56, 0x57], 3 + 4), // PUSH EBX; PUSH ESI; PUSH EDI
        ([0x53, 0x83, 0xec], 4 + 5), // PUSH EBX; SUB ESP,imm8
        ([0x56, 0x83, 0xec], 4 + 5), // PUSH ESI; SUB ESP,imm8
        ([0x57, 0x83, 0xec], 4 + 5), // PUSH EDI; SUB ESP,imm8
        ([0x83, 0xec], 3 + 4),       // SUB ESP,imm8
        ([0x81, 0xec], 6 + 7),       // SUB ESP,imm32
    ];

    static unsafe Span<byte> GetMethodInstructions(MethodInfo method)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        byte* p = SkipBranches((byte*)method.MethodHandle.GetFunctionPointer());
        return Unwind.GetMethodInstructions(p);
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

            using (var trampolineScope = MemoryProtectionScope.Create(
                trampoline,
                NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute))
            {
                code.CopyTo(trampoline);
                trampoline[code.Length] = Ret;
                trampolineScope.FlushInstructions();
            }

            instructions = instructions[..JmpRel32Size];
        }
        else
        {
            instructions = instructions[..patchSize];
        }

        using var scope = MemoryProtectionScope.Create(
            instructions,
            NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute);

        if (!trampoline.IsEmpty)
        {
            nint offset = Unsafe.ByteOffset(
                ref MemoryMarshal.GetReference(instructions),
                ref MemoryMarshal.GetReference(trampoline));
            int displacement = (int)(offset - JmpRel32Size);
            instructions[0] = JmpRel32;
            MemoryMarshal.Write(instructions[1..], ref displacement);
        }
        else
        {
            code.CopyTo(instructions);
            instructions[code.Length] = Ret;
        }

        scope.FlushInstructions();
        return PatchResult.Success;
    }
}
