// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS9191 // The 'ref' modifier for an argument corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Intrinsic adapter for Windows OS and ARM-based 64-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class AdapterWindowsArm64 : AdapterArm64
{
    public override bool IsFeatureSupported(MachineCodeIntrinsicFeature feature)
    {
        return feature switch
        {
            // Advanced SIMD is a baseline requirement for Windows on ARM64.
            MachineCodeIntrinsicFeature.AdvSimd => true,
            _ => base.IsFeatureSupported(feature)
        };
    }

    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        // Every ARM64 instruction is four bytes long.
        if ((code.Length & (sizeof(uint) - 1)) != 0)
            return PatchResult.InvalidAlignment;

        var instructions = GetMethodInstructions(method, out var entryPoint);
        if (!IsSupportedPrologue(instructions))
            return PatchResult.UnexpectedPrologue;

        var patchCode = MemoryMarshal.Cast<byte, uint>(code);

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
            result = ApplyPatch(instructions, entryPoint, patchCode);
        }
        return result;
    }

    static bool IsSupportedPrologue(ReadOnlySpan<uint> instructions)
    {
        if (instructions.Length < 1)
            return false;

        uint instruction = instructions[0];
        return
            // STP X29, X30, [SP, #-imm]!
            (instruction & 0xffc07fff) == 0xa9807bfd ||
            // SUB SP, SP, #imm{, LSL #12}
            (instruction & 0xff8003ff) == 0xd10003ff;
    }

    static unsafe Span<uint> GetMethodInstructions(MethodInfo method, out Span<uint> entryPoint)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        uint* p0 = (uint*)method.MethodHandle.GetFunctionPointer();
        uint* p = SkipBranches(p0);
        entryPoint = p0 != p ? new(p0, 1) : [];

        // Get the exact method instruction boundaries.
        var runtimeFunction = (NativeMethods.RuntimeFunctionArm64*)NativeMethods.RtlLookupFunctionEntry(p, out void* imageBase, null);
        if (runtimeFunction == null)
            return [];

        uint unwindData = runtimeFunction->UnwindData;
        uint functionLength =
            (unwindData & 3) switch
            {
                // Unpacked .xdata format 0
                0 => *(uint*)((byte*)imageBase + unwindData) & 0x3ffff,

                // Packed unwind formats 1 and 2
                1 or 2 => (unwindData >> 2) & 0x7ff,

                _ => 0
            };

        if (functionLength > int.MaxValue)
            return [];

        byte* functionStart = (byte*)imageBase + runtimeFunction->BeginAddress;
        byte* functionEnd = functionStart + (functionLength << 2);
        if ((byte*)p < functionStart || (byte*)p >= functionEnd)
            return [];

        return new(p, (int)functionLength);
    }

    static unsafe PatchResult ApplyPatch(Span<uint> instructions, Span<uint> entryPoint, ReadOnlySpan<uint> code)
    {
        int patchSize = checked(code.Length + 1 /* RET */);

        var trampoline = Span<uint>.Empty;
        uint branchDisplacement = 0;
        if (patchSize > instructions.Length)
        {
            var redirection = entryPoint.IsEmpty ? instructions[..1] : entryPoint;
            ref uint instruction = ref MemoryMarshal.GetReference(redirection);
            void* target = Unsafe.AsPointer(ref instruction);

            if (!TrampolineAllocator.TryAllocateNear(target, patchSize, (nuint)BranchMaximumDistance, out trampoline))
                return PatchResult.NoSpace;

            nint entryOffset = Unsafe.ByteOffset(
                ref instruction,
                ref MemoryMarshal.GetReference(trampoline));
            if (!TryEncodeBranch(entryOffset, out branchDisplacement))
                return PatchResult.NoSpace;

            using var trampolineScope = VirtualProtectionScope.Create(trampoline, NativeMethods.PageProtect.ExecuteReadWrite);
            code.CopyTo(trampoline);
            trampoline[code.Length] = Ret;
            trampolineScope.FlushInstructions();

            instructions = redirection;
        }
        else
        {
            instructions = instructions[..patchSize];
        }

        // Temporarily allow memory modification in order to apply the intrinsic code.
        using var scope = VirtualProtectionScope.Create(instructions, NativeMethods.PageProtect.ExecuteReadWrite);

        if (!trampoline.IsEmpty)
        {
            // Redirect invocations from the entry veneer, or directly from the method entry
            // when the runtime does not expose a separate veneer.
            instructions[0] = B | branchDisplacement;
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
