// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
sealed class AdapterLinuxArm64 : AdapterArm64
{
    protected override unsafe void GetMethodInstructions(
        MethodInfo method,
        out Span<uint> instructions,
        out Span<uint> entryPoint,
        out Span<nuint> entryPointTarget)
    {
        uint* p = GetMethodCodePointer(method, out entryPoint, out entryPointTarget);
        if (((nuint)p & (sizeof(uint) - 1)) != 0)
        {
            instructions = [];
            return;
        }
        instructions = MemoryMarshal.Cast<byte, uint>(Unwind.GetMethodInstructions((byte*)p));
    }

    protected override bool IsWriteAllowed<T>(Span<T> span) => true;

    protected override Span<uint> AllocateTrampoline(int count)
    {
        return TrampolineAllocator.Allocate<uint>(count);
    }

    protected override unsafe bool TryAllocateTrampolineNear(void* target, int count, out Span<uint> trampoline)
    {
        return TrampolineAllocator.TryAllocateNear(target, count, (nuint)BranchMaximumDistance, out trampoline);
    }

    protected override void WriteCode(Span<uint> destination, ReadOnlySpan<uint> code)
    {
        using var scope = CreateWriteScope(destination);
        code.CopyTo(destination);
        destination[code.Length] = Ret;
        scope.FlushInstructions();
    }

    protected override void WriteBranch(Span<uint> destination, uint displacement)
    {
        using var scope = CreateWriteScope(destination);
        destination[0] = B | displacement;
        scope.FlushInstructions();
    }

    protected override void WriteAddress(Span<nuint> destination, nuint address)
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
