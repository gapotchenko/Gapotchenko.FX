// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

static class OperationResults
{
    /// <summary>
    /// Indicates the result of managed code.
    /// </summary>
    public const int Managed = -1;

    /// <summary>
    /// Indicates the result of machine code.
    /// </summary>
    public const int Intrinsic = 31;

    /// <summary>
    /// Indicates the result of a non-leaf machine code.
    /// </summary>
    public static int NonLeafIntrinsic { get; } = GetNonLeafIntrinsic();

    static int GetNonLeafIntrinsic()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && RuntimeInformation.ProcessArchitecture is Architecture.X64 or Architecture.Arm64 ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && RuntimeInformation.ProcessArchitecture is Architecture.X64 or Architecture.Arm64 ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.ProcessArchitecture is Architecture.X86 or Architecture.X64 or Architecture.Arm64)
        {
            // Platforms with unwind support can handle non-leaf intrinsic functions.
            return Intrinsic;
        }
        else
        {
            return Managed;
        }
    }
}
