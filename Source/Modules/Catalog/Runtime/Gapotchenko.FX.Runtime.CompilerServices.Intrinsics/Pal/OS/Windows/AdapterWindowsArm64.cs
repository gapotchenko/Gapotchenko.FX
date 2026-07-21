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

        var methodInstructions = GetMethodInstructions(method);
        if (!IsSupportedPrologue(methodInstructions))
            return PatchResult.UnexpectedPrologue;

        var patchInstructions = MemoryMarshal.Cast<byte, uint>(code);

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
            methodInstructions[patchInstructions.Length] = 0xd65f03c0;

            scope.FlushInstructions();
        }

        return PatchResult.Success;
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

    static unsafe Span<uint> GetMethodInstructions(MethodInfo method)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        uint* p = (uint*)method.MethodHandle.GetFunctionPointer();
        p = SkipBranches(p);

        // Get instruction boundaries.
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
}
