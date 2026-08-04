// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Tests;

[TestClass]
public class IntrisincTests
{
    [TestMethod]
    public void Intrinsics_Capabilities()
    {
        Assert.AreEqual(GetExpectedCapabilities(), Intrinsics.Capabilities);
    }

    static IntrinsicCapabilities GetExpectedCapabilities()
    {
        // Corresponds to the support matrix table in README.md file
        // of Gapotchenko.FX.Runtime.CompilerServices.Intrinsics module.

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && RuntimeInformation.ProcessArchitecture is Architecture.X86 or Architecture.X64 or Architecture.Arm64 ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && RuntimeInformation.ProcessArchitecture is Architecture.X64 or Architecture.Arm64 ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.ProcessArchitecture is Architecture.X86 or Architecture.X64 or Architecture.Arm or Architecture.Arm64)
        {
            var capabilities = IntrinsicCapabilities.FeatureSupport;
            if (RuntimeInformation.ProcessArchitecture is not Architecture.Arm)
                capabilities |= IntrinsicCapabilities.Compilation;
            return capabilities;
        }
        else
        {
            return IntrinsicCapabilities.None;
        }
    }
}
