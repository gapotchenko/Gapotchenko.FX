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
    public static int Intrinsic { get; } = GetIntrinsic();

    static int GetIntrinsic()
    {
        return IntrinsicCapabilities.Compilation ? 31 : Managed;
    }

    /// <summary>
    /// Indicates the result of a non-leaf machine code.
    /// </summary>
    public static int NonLeafIntrinsic { get; } = GetNonLeafIntrinsic();

    static int GetNonLeafIntrinsic()
    {
        if (RuntimeInformation.ProcessArchitecture is Architecture.Arm)
        {
            // Platforms without unwind support cannot handle non-leaf intrinsic functions.
            return Managed;
        }
        else
        {
            return Intrinsic;
        }
    }
}
