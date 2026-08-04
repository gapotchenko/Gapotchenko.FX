// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

static class IntrinsicCapabilities
{
    static IntrinsicCapabilities()
    {
        // Corresponds to the support matrix table in README.md file
        // of Gapotchenko.FX.Runtime.CompilerServices.Intrinsics module.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && RuntimeInformation.ProcessArchitecture is Architecture.X86 or Architecture.X64 or Architecture.Arm64 ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && RuntimeInformation.ProcessArchitecture is Architecture.X64 or Architecture.Arm64 ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.ProcessArchitecture is Architecture.X86 or Architecture.X64 or Architecture.Arm or Architecture.Arm64)
        {
            FeatureDetection = true;
            Compilation = RuntimeInformation.ProcessArchitecture is not Architecture.Arm;
        }
    }

    public static bool Compilation { get; }

    public static bool FeatureDetection { get; }
}
