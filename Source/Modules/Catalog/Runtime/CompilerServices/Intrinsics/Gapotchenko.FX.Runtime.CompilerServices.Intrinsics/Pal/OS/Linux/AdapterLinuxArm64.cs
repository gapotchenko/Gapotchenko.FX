// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Pal.Formats.Dwarf;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
sealed class AdapterLinuxArm64 : AdapterArm64
{
    protected override PatchResult ValidateCode(ReadOnlySpan<uint> code) =>
        UnwindArm64.Analyze(code, out _) == UnwindAnalysisResult.Unsupported ?
        PatchResult.UnsupportedUnwindPrologue :
        PatchResult.Success;

    protected override bool RequiresTrampoline(ReadOnlySpan<uint> code) =>
        UnwindArm64.Analyze(code, out _) == UnwindAnalysisResult.Supported;

    protected override int GetTrampolineAllocationCount(ReadOnlySpan<uint> code)
    {
        var result = UnwindArm64.Analyze(code, out var info);
        return result switch
        {
            UnwindAnalysisResult.Leaf => base.GetTrampolineAllocationCount(code),
            UnwindAnalysisResult.Supported => checked(
                code.Length + 1 +
                (DwarfUnwindArm64.GetSize(code, info) + sizeof(uint) - 1) / sizeof(uint) +
                1),
            _ => throw new InvalidOperationException("The intrinsic has an unsupported Linux ARM64 unwind prologue.")
        };
    }

    protected override unsafe void GetMethodInstructions(
        MethodInfo method,
        out Span<uint> instructions,
        out Span<uint> entryPoint,
        out Span<nuint> entryPointTarget,
        out bool hasExactBoundaries)
    {
        hasExactBoundaries = false;
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
        using var scope = CreateWriteScope(destination);
        code.CopyTo(destination);
        destination[code.Length] = InstructionsArm64.Ret;
        scope.FlushInstructions();
    }

    protected override void WriteBranch(Span<uint> destination, uint displacement)
    {
        using var scope = CreateWriteScope(destination);
        destination[0] = InstructionsArm64.B | displacement;
        scope.FlushInstructions();
    }

    protected override unsafe void WriteTrampoline(Span<uint> destination, ReadOnlySpan<uint> code)
    {
        var result = UnwindArm64.Analyze(code, out var info);
        if (result == UnwindAnalysisResult.Leaf)
        {
            WriteCode(destination, code);
            return;
        }
        if (result != UnwindAnalysisResult.Supported)
            throw new InvalidOperationException("The intrinsic has an unsupported Linux ARM64 unwind prologue.");

        int codeCount = checked(code.Length + 1);
        int unwindSize = DwarfUnwindArm64.GetSize(code, info);
        int unwindCount = checked((unwindSize + sizeof(uint) - 1) / sizeof(uint));
        int allocationCount = checked(codeCount + unwindCount + 1);
        destination = destination[..allocationCount];

        ref uint baseAddress = ref MemoryMarshal.GetReference(destination);
        var unwindDestination = MemoryMarshal.AsBytes(destination.Slice(codeCount, unwindCount));
        using (var scope = CreateWriteScope(destination))
        {
            code.CopyTo(destination);
            destination[code.Length] = InstructionsArm64.Ret;
            destination[codeCount..].Clear();
            DwarfUnwindArm64.Write(
                unwindDestination,
                code,
                info,
                Unsafe.AsPointer(ref baseAddress));
            scope.FlushInstructions();
        }

        NativeMethods.RegisterFrame(Unsafe.AsPointer(ref MemoryMarshal.GetReference(unwindDestination)));
    }

    protected override void WriteLongBranch(Span<uint> destination, uint adrp, uint add)
    {
        using var scope = CreateWriteScope(destination);
        destination[0] = adrp;
        destination[1] = add;
        destination[2] = InstructionsArm64.BrX16;
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
