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
            MachineCodeIntrinsicFeature.Lzcnt => Lzcnt.IsSupported,
#else
            MachineCodeIntrinsicFeature.Popcnt => (FeatureDetection.Cpuid_01h_Ecx & (1 << 23)) != 0,
            MachineCodeIntrinsicFeature.Lzcnt => (FeatureDetection.Cpuid_80000001h_Ecx & (1 << 5)) != 0,
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

            uint maxExtendedFunctionId = (uint)Cpuid(unchecked((int)0x80000000), 0).Eax;
            if (maxExtendedFunctionId >= 0x80000001)
                (_, _, Cpuid_80000001h_Ecx, _) = Cpuid(unchecked((int)0x80000001), 0);
        }

        public static int Cpuid_01h_Ecx { get; }

        public static int Cpuid_80000001h_Ecx { get; }

        public static unsafe (int Eax, int Ebx, int Ecx, int Edx) Cpuid(int functionId, int subFunctionId)
        {
            int* cpuInfo = stackalloc int[4];
            CallCpuid(cpuInfo, functionId, subFunctionId);
            return (cpuInfo[0], cpuInfo[1], cpuInfo[2], cpuInfo[3]);
        }

        // x86
        [MachineCodeIntrinsic(
            Architecture.X86,
            0x53,                    // PUSH EBX
            0x56,                    // PUSH ESI
            0x8b, 0xf1,              // MOV ESI, ECX
            0x8b, 0xc2,              // MOV EAX, EDX
            0x8b, 0x4c, 0x24, 0x0c,  // MOV ECX, [ESP+12]
            0x0f, 0xa2,              // CPUID
            0x89, 0x06,              // MOV [ESI], EAX
            0x89, 0x5e, 0x04,        // MOV [ESI+4], EBX
            0x89, 0x4e, 0x08,        // MOV [ESI+8], ECX
            0x89, 0x56, 0x0c,        // MOV [ESI+12], EDX
            0x5e,                    // POP ESI
            0x5b,                    // POP EBX
            0xc2, 0x04, 0x00,        // RET 4
            SupportedOSPlatforms = ["windows"])]
        [MachineCodeIntrinsic(
            Architecture.X86,
            0x53,                    // PUSH EBX
            0x56,                    // PUSH ESI
            0x8b, 0x74, 0x24, 0x0c,  // MOV ESI, [ESP+12]
            0x8b, 0x44, 0x24, 0x10,  // MOV EAX, [ESP+16]
            0x8b, 0x4c, 0x24, 0x14,  // MOV ECX, [ESP+20]
            0x0f, 0xa2,              // CPUID
            0x89, 0x06,              // MOV [ESI], EAX
            0x89, 0x5e, 0x04,        // MOV [ESI+4], EBX
            0x89, 0x4e, 0x08,        // MOV [ESI+8], ECX
            0x89, 0x56, 0x0c,        // MOV [ESI+12], EDX
            0x5e,                    // POP ESI
            0x5b,                    // POP EBX
            SupportedOSPlatforms = ["linux"])]
        // x64
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
            0x5b,                    // POP RBX
            SupportedOSPlatforms = ["windows"])]
        [MachineCodeIntrinsic(
            Architecture.X64,
            0x53,                    // PUSH RBX
            0x49, 0x89, 0xf8,        // MOV R8, RDI
            0x89, 0xf0,              // MOV EAX, ESI
            0x89, 0xd1,              // MOV ECX, EDX
            0x0f, 0xa2,              // CPUID
            0x41, 0x89, 0x00,        // MOV [R8], EAX
            0x41, 0x89, 0x58, 0x04,  // MOV [R8+4], EBX
            0x41, 0x89, 0x48, 0x08,  // MOV [R8+8], ECX
            0x41, 0x89, 0x50, 0x0c,  // MOV [R8+12], EDX
            0x5b,                    // POP RBX
            SupportedOSPlatforms = ["linux"])]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static unsafe void CallCpuid(int* cpuInfo, int functionId, int subFunctionId)
        {
            cpuInfo[0] = 0;
            cpuInfo[1] = 0;
            cpuInfo[2] = 0;
            cpuInfo[3] = 0;
        }
    }

#endif

    protected static unsafe byte* SkipBranches(byte* p)
    {
        for (; ; )
        {
            switch (*p)
            {
                case 0xe9: // JMP rel32
                    p += *(int*)(p + 1) + 5;
                    break;

                case 0xeb: // JMP rel8
                    p += *(sbyte*)(p + 1) + 2;
                    break;

                case 0xff when p[1] == 0x25: // JMP r/m (absolute on x86, RIP-relative on x64)
                    if (IntPtr.Size == 8)
                        p = *(byte**)(p + *(int*)(p + 2) + 6);
                    else
                        p = **(byte***)(p + 2);
                    break;

                default:
                    return p;
            }
        }
    }

    protected const byte Ret = 0xc3;
    protected const int JmpRel32 = 0xe9;
    protected const int JmpRel32Size = 5;
}
