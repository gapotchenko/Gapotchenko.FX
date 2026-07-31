// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Reflection;

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
#if !NET
            MachineCodeIntrinsicFeature.Crc32 => NativeMethods.IsProcessorFeaturePresent(
                NativeMethods.ProcessorFeature.ArmV8Crc32InstructionsAvailable),
#endif
            _ => base.IsFeatureSupported(feature)
        };
    }

    protected override unsafe void GetMethodInstructions(
        MethodInfo method,
        out Span<uint> instructions,
        out Span<uint> entryPoint,
        out Span<nuint> entryPointTarget,
        out bool hasExactBoundaries)
    {
        hasExactBoundaries = true;
        uint* p = GetMethodCodePointer(method, out entryPoint, out entryPointTarget);

        // Get the exact method instruction boundaries.
        var runtimeFunction = (NativeMethods.RuntimeFunctionArm64*)NativeMethods.RtlLookupFunctionEntry(p, out void* imageBase, null);
        if (runtimeFunction == null)
        {
            instructions = [];
            return;
        }

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
        {
            instructions = [];
            return;
        }

        byte* functionStart = (byte*)imageBase + runtimeFunction->BeginAddress;
        byte* functionEnd = functionStart + (functionLength << 2);
        if ((byte*)p < functionStart || (byte*)p >= functionEnd)
        {
            instructions = [];
            return;
        }

        instructions = new(p, (int)functionLength);
    }

    protected override bool IsWriteAllowed<T>(Span<T> span) => true;

    protected override Span<uint> AllocateTrampoline(int count)
    {
        return TrampolineAllocator.Allocate<uint>(count);
    }

    protected override unsafe bool TryAllocateTrampolineNear(
        void* target,
        int count,
        nuint maximumDistance,
        out Span<uint> trampoline)
    {
        return TrampolineAllocator.TryAllocateNear(target, count, maximumDistance, out trampoline);
    }

    protected override void WriteCode(Span<uint> destination, ReadOnlySpan<uint> code)
    {
        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
        code.CopyTo(destination);
        destination[code.Length] = Ret;
        scope.FlushInstructions();
    }

    protected override void WriteBranch(Span<uint> destination, uint displacement)
    {
        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
        destination[0] = B | displacement;
        scope.FlushInstructions();
    }

    protected override void WriteLongBranch(Span<uint> destination, uint adrp, uint add)
    {
        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
        destination[0] = adrp;
        destination[1] = add;
        destination[2] = BrX16;
        scope.FlushInstructions();
    }

    protected override void WriteAddress(Span<nuint> destination, nuint address)
    {
        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
        destination[0] = address;
    }
}
