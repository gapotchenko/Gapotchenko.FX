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

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
sealed class AdapterMacOSX64 : AdapterX64
{
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        var instructions = GetMethodInstructions(method, out var entryPoint);
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
            result = ApplyPatch(instructions, entryPoint, code);
        }
        return result;
    }

    // The table defines a conservative minimum method body extent:
    // complete prologue plus the shortest matching restoration/return sequence.
    static readonly (byte[] Instructions, int Size)[] m_SupportedPrologues =
    [
        ([0x41, 0x57, 0x53, 0x48, 0x83, 0xec], 7 + 8),
        ([0x48, 0x83, 0xec], 4 + 5),
        ([0x48, 0x81, 0xec], 7 + 8),
        ([0x53, 0x48, 0x83, 0xec], 5 + 6),
        ([0x55, 0x48, 0x8b, 0xec], 4 + 2), // .NET 10 x64, tier 0
        ([0x55, 0x48, 0x89, 0xe5], 4 + 2),
        ([0x55, 0x48, 0x83, 0xec], 5 + 6),
        ([0x55, 0x41, 0x57], 3 + 4),
        ([0x56, 0x48, 0x83, 0xec], 5 + 6),
        ([0x57, 0x48, 0x83, 0xec], 5 + 6)
    ];

    static unsafe PatchResult ApplyPatch(Span<byte> instructions, Span<byte> entryPoint, ReadOnlySpan<byte> code)
    {
        int patchSize = code.Length + 1 /* RET */;

        var trampoline = Span<byte>.Empty;
        if (patchSize > instructions.Length)
        {
            var redirection = entryPoint.IsEmpty ? instructions : entryPoint;
            if (redirection.Length < JmpAbs64Size)
                return PatchResult.NoSpace;

            redirection = redirection[..JmpAbs64Size];
            if (!MemoryMap.IsWriteAllowed(redirection))
                return PatchResult.WriteProtected;

            trampoline = TrampolineAllocator.Allocate(patchSize);

            using (var trampolineScope = JitWriteProtectionScope.Create(trampoline))
            {
                code.CopyTo(trampoline);
                trampoline[code.Length] = Ret;
                trampolineScope.FlushInstructions();
            }

            instructions = redirection;
        }
        else
        {
            instructions = instructions[..patchSize];
            if (!MemoryMap.IsWriteAllowed(instructions))
                return PatchResult.WriteProtected;
        }

        using var scope = MemoryProtectionScope.Create(
            instructions,
            NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute);

        if (!trampoline.IsEmpty)
        {
            JmpAbs64.CopyTo(instructions);
            nint address = (nint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(trampoline));
            MemoryMarshal.Write(instructions[JmpAbs64.Length..], ref address);
        }
        else
        {
            code.CopyTo(instructions);
            instructions[code.Length] = Ret;
        }

        scope.FlushInstructions();
        return PatchResult.Success;
    }

    static unsafe Span<byte> GetMethodInstructions(MethodInfo method, out Span<byte> entryPoint)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        byte* p0 = (byte*)method.MethodHandle.GetFunctionPointer();
        byte* p = SkipBranches(p0);

        // .NET x64 uses a 20-byte precode stub beginning with an indirect jump,
        // followed by "MOV R10,[RIP+disp32]". It safely accommodates JmpAbs64.
        if (p0 != p &&
            p0[0] == 0xff && p0[1] == 0x25 &&
            p0[6] == 0x4c && p0[7] == 0x8b && p0[8] == 0x15)
        {
            entryPoint = new(p0, JmpAbs64Size);
        }
        else
        {
            entryPoint = [];
        }

        return Unwind.GetMethodInstructions(p);
    }
}
