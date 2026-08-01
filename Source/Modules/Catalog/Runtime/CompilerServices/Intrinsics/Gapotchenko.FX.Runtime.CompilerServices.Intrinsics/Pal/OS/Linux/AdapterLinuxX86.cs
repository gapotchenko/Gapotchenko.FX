// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Pal.Formats.Dwarf;
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
    protected override PatchResult ValidateCode(ReadOnlySpan<byte> code)
    {
        return DwarfUnwindX86.Analyze(code, out _) == UnwindAnalysisResult.Unsupported ?
            PatchResult.UnsupportedUnwindPrologue :
            PatchResult.Success;
    }

    protected override bool RequiresTrampoline(ReadOnlySpan<byte> code) =>
        DwarfUnwindX86.Analyze(code, out _) == UnwindAnalysisResult.Supported;

    protected override int GetTrampolineAllocationSize(ReadOnlySpan<byte> code)
    {
        int codeSize = checked(code.Length + 1);
        var result = DwarfUnwindX86.Analyze(code, out var info);
        if (result == UnwindAnalysisResult.Leaf)
            return codeSize;
        if (result != UnwindAnalysisResult.Supported)
            throw new InvalidOperationException("The intrinsic has an unsupported Linux x86 unwind prologue.");

        int unwindOffset = MemoryArithmetics.Align4(codeSize);
        return checked(unwindOffset + DwarfUnwindX86.GetSize(code, info) + sizeof(uint));
    }

    protected override unsafe void GetMethodInstructions(
        MethodInfo method,
        out Span<byte> instructions,
        out bool hasExactBoundaries)
    {
        hasExactBoundaries = false;
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        byte* p = SkipBranches((byte*)method.MethodHandle.GetFunctionPointer());
        instructions = Unwind.GetMethodInstructions(p);
    }

    protected override bool IsWriteAllowed(Span<byte> span) => true;

    protected override Span<byte> AllocateTrampoline(int size)
    {
        return TrampolineAllocator.Allocate(size);
    }

    protected override void WriteCode(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        WriteCodeCore(destination, code);
    }

    protected override unsafe void WriteTrampoline(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        var result = DwarfUnwindX86.Analyze(code, out var info);
        if (result == UnwindAnalysisResult.Leaf)
        {
            WriteCodeCore(destination, code);
            return;
        }
        if (result != UnwindAnalysisResult.Supported)
            throw new InvalidOperationException("The intrinsic has an unsupported Linux x86 unwind prologue.");

        int codeSize = checked(code.Length + 1);
        int unwindOffset = MemoryArithmetics.Align4(codeSize);
        int unwindSize = DwarfUnwindX86.GetSize(code, info);
        destination = destination[..checked(unwindOffset + unwindSize + sizeof(uint))];

        ref byte baseAddress = ref MemoryMarshal.GetReference(destination);
        var unwindDestination = destination.Slice(unwindOffset, unwindSize);
        using (var scope = CreateWriteScope(destination))
        {
            code.CopyTo(destination);
            destination[code.Length] = InstructionsX86.Ret;
            destination[codeSize..unwindOffset].Clear();
            DwarfUnwindX86.Write(unwindDestination, code, info, Unsafe.AsPointer(ref baseAddress));
            destination[(unwindOffset + unwindSize)..].Clear();
            scope.FlushInstructions();
        }

        NativeMethods.RegisterFrame(Unsafe.AsPointer(ref MemoryMarshal.GetReference(unwindDestination)));
    }

    static void WriteCodeCore(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        using var scope = CreateWriteScope(destination);
        code.CopyTo(destination);
        destination[code.Length] = InstructionsX86.Ret;
        scope.FlushInstructions();
    }

    protected override void WriteRedirection(Span<byte> destination, Span<byte> trampoline)
    {
        nint offset = Unsafe.ByteOffset(
            ref MemoryMarshal.GetReference(destination),
            ref MemoryMarshal.GetReference(trampoline));
        int displacement = (int)(offset - InstructionsX86.JmpRel32Size);

        using var scope = CreateWriteScope(destination);
        destination[0] = InstructionsX86.JmpRel32;
        MemoryMarshal.Write(destination[1..], ref displacement);
        scope.FlushInstructions();
    }

    static MemoryProtectionScope CreateWriteScope(Span<byte> span)
    {
        return MemoryProtectionScope.Create(
            span,
            NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute);
    }
}
