// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
sealed class AdapterLinuxX64 : AdapterX64
{
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        var instructions = GetMethodInstructions(method);
        if (!Util.HasPrologue(instructions, m_SupportedPrologues))
            return PatchResult.UnexpectedPrologue;

        int patchSize = code.Length + 1 /* RET */;
        if (patchSize > instructions.Length)
            return PatchResult.NoSpace;

        instructions = instructions[..patchSize];

#if TFF_CER
        // Ensure that code changes are atomic by using the constrained execution region.
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
#endif
        {
            using var scope = MemoryProtectionScope.Create(instructions, NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute);

            // Put the intrinsic code.
            code.CopyTo(instructions);

            // End the method with a RET instruction.
            instructions[code.Length] = 0xc3;
        }

        return PatchResult.Success;
    }

    static readonly byte[][] m_SupportedPrologues =
    [
        [0x48, 0x83, 0xec],
        [0x48, 0x81, 0xec],
        [0x53, 0x48, 0x83, 0xec],
        [0x55, 0x48, 0x89, 0xe5],
        [0x55, 0x48, 0x83, 0xec],
        [0x55, 0x41, 0x57],
        [0x56, 0x48, 0x83, 0xec],
        [0x57, 0x48, 0x83, 0xec]
    ];

    static unsafe Span<byte> GetMethodInstructions(MethodInfo method)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        byte* p = SkipBranches((byte*)method.MethodHandle.GetFunctionPointer());
        return Unwind.GetMethodInstructions(p);
    }
}
