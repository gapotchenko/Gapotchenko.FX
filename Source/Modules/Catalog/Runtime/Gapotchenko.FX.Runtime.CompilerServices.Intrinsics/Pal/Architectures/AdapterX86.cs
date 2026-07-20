// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET
using System.Runtime.Intrinsics.X86;
#endif

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class AdapterX86 : Adapter
{
    public override bool IsFeatureSupported(MachineCodeIntrinsicFeature feature)
    {
        return feature switch
        {
#if NET
            MachineCodeIntrinsicFeature.Popcnt => Popcnt.IsSupported,
#else
            MachineCodeIntrinsicFeature.Popcnt => (FeatureDetection.Cpuid_01h_Ecx & (1 << 23)) != 0,
#endif
            _ => base.IsFeatureSupported(feature)
        };
    }

#if !NET

    protected static class FeatureDetection
    {
        static FeatureDetection()
        {
            Intrinsics.InitializeType(typeof(FeatureDetection));

            (_, _, Cpuid_01h_Ecx, _) = Cpuid(1, 0);
        }

        public static int Cpuid_01h_Ecx { get; }

        public static unsafe (int Eax, int Ebx, int Ecx, int Edx) Cpuid(int functionId, int subFunctionId)
        {
            int* cpuInfo = stackalloc int[4];
            Cpuid(cpuInfo, functionId, subFunctionId);
            return (cpuInfo[0], cpuInfo[1], cpuInfo[2], cpuInfo[3]);
        }

        [MachineCodeIntrinsic(
            Architecture.X64,
            0x53,                    // PUSH RBX
            0x49, 0x89, 0xc9,        // MOV R9, RCX
            0x89, 0xd0,              // MOV EAX, EDX
            0x44, 0x89, 0xc1,        // MOV ECX, R8D
            0x0f, 0xa2,              // CPUID
            0x41, 0x89, 0x01,        // MOV [R9], EAX
            0x41, 0x89, 0x59, 0x04,  // MOV [R9+4], EBX
            0x41, 0x89, 0x49, 0x08,  // MOV [R9+8], ECX
            0x41, 0x89, 0x51, 0x0c,  // MOV [R9+12], EDX
            0x5b)]                   // POP RBX
        [MethodImpl(MethodImplOptions.NoInlining)]
        static unsafe void Cpuid(int* cpuInfo, int functionId, int subFunctionId)
        {
            cpuInfo[0] = 0;
            cpuInfo[1] = 0;
            cpuInfo[2] = 0;
            cpuInfo[3] = 0;

            // Reserve space for the intrinsic code.
            Fn.Ignore(Fn.Empty);
            Fn.Ignore(Fn.Empty);
            Fn.Ignore(Fn.Empty);
            Fn.Ignore(Fn.Empty);
            Fn.Ignore(Fn.Empty);
            Fn.Ignore(Fn.Empty);
            Fn.Ignore(Fn.Empty);
            Fn.Ignore(Fn.Empty);
        }
    }

#endif
}
