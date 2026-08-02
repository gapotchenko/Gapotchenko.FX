// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.Arm.Arm32;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
sealed class AdapterLinuxArm32 : AdapterArm32
{
    protected override PatchResult ValidateCode(ReadOnlySpan<ushort> code)
    {
        return UnwindLinuxArm32.Analyze(code) == UnwindAnalysisResult.Unsupported ?
            PatchResult.UnsupportedUnwindPrologue :
            PatchResult.Success;
    }

    protected override unsafe void GetMethodInstructions(
        MethodInfo method,
        out Span<ushort> instructions,
        out Span<ushort> entryPoint,
        out Span<uint> entryPointTarget)
    {
        ushort* p = GetMethodCodePointer(method, out entryPoint, out entryPointTarget);
        if (((nuint)p & (sizeof(ushort) - 1)) != 0)
        {
            instructions = [];
            return;
        }
        instructions = MemoryMarshal.Cast<byte, ushort>(Unwind.GetMethodInstructions((byte*)p));
    }

    protected override bool IsWriteAllowed<T>(Span<T> span) => true;

    protected override Span<ushort> AllocateTrampoline(int count)
    {
        return TrampolineAllocator.Allocate<ushort>(count);
    }

    protected override unsafe bool TryAllocateTrampolineNear(void* target, int count, out Span<ushort> trampoline)
    {
        return TrampolineAllocator.TryAllocateNear(target, count, (nuint)InstructionsArm32.BranchMaximumDistance, out trampoline);
    }

    protected override void WriteCode(Span<ushort> destination, ReadOnlySpan<ushort> code)
    {
        using var scope = CreateWriteScope(destination);
        code.CopyTo(destination);
        destination[code.Length] = InstructionsArm32.BxLr;
        scope.FlushInstructions();
    }

    protected override void WriteBranch(Span<ushort> destination, ushort first, ushort second)
    {
        using var scope = CreateWriteScope(destination);
        destination[0] = first;
        destination[1] = second;
        scope.FlushInstructions();
    }

    protected override void WriteAddress(Span<uint> destination, uint address)
    {
        using var scope = CreateWriteScope(destination);
        destination[0] = address;
    }

    static MemoryProtectionScope CreateWriteScope<T>(Span<T> span) where T : struct
    {
        return MemoryProtectionScope.Create(
            span,
            NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute);
    }
}
