// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

partial class EdgeCaseOperations
{
    public static class Unwind
    {
        static Unwind()
        {
            Intrinsics.InitializeType(typeof(Unwind));
        }

        [MachineCodeIntrinsic(
            Architecture.Arm64,
            0xff, 0x43, 0x00, 0xd1,  // SUB SP,SP,#16
            0xe0, 0x03, 0x80, 0x52,  // MOV W0,31
            0xff, 0x43, 0x00, 0x91,  // ADD SP,SP,#16
            SupportedOSPlatforms = ["windows", "linux", "macos"])]
        [MachineCodeIntrinsic(
            Architecture.Arm,
            0x84, 0xb0,  // SUB SP,#16
            0x1f, 0x20,  // MOVS R0,31
            0x04, 0xb0,  // ADD SP,#16
            SupportedOSPlatforms = ["linux"])]
        [MachineCodeIntrinsic(
            Architecture.X64,
            0x48, 0x83, 0xec, 0x28,        // SUB RSP,40
            0xb8, 0x1f, 0x00, 0x00, 0x00,  // MOV EAX,31
            0x48, 0x83, 0xc4, 0x28,        // ADD RSP,40
            SupportedOSPlatforms = ["windows", "linux", "macos"])]
        [MachineCodeIntrinsic(
            Architecture.X86,
            0x83, 0xec, 0x10,              // SUB ESP,16
            0xb8, 0x1f, 0x00, 0x00, 0x00,  // MOV EAX,31
            0x83, 0xc4, 0x10,              // ADD ESP,16
            SupportedOSPlatforms = ["windows", "linux"])]
        [MethodImpl(Intrinsics.MethodImplOptions)]
        public static int StackAllocationIntrinsic() => OperationResults.Managed;

        [MachineCodeIntrinsic(
            Architecture.Arm64,
            0xfd, 0x7b, 0xbe, 0xa9,  // STP X29,LR,[SP,#-32]!
            0xfd, 0x03, 0x00, 0x91,  // MOV X29,SP
            0xe0, 0x03, 0x80, 0x52,  // MOV W0,31
            0xfd, 0x7b, 0xc2, 0xa8,  // LDP X29,LR,[SP],#32
            SupportedOSPlatforms = ["windows", "linux", "macos"])]
        [MachineCodeIntrinsic(
            Architecture.Arm,
            0x80, 0xb5,  // PUSH {R7,LR}
            0x6f, 0x46,  // MOV R7,SP
            0x1f, 0x20,  // MOVS R0,31
            0x80, 0xbd,  // POP {R7,PC}
            SupportedOSPlatforms = ["linux"])]
        [MachineCodeIntrinsic(
            Architecture.X64,
            0x55,                          // PUSH RBP
            0x48, 0x89, 0xe5,              // MOV RBP,RSP
            0xb8, 0x1f, 0x00, 0x00, 0x00,  // MOV EAX,31
            0x5d,                          // POP RBP
            SupportedOSPlatforms = ["windows", "linux", "macos"])]
        [MachineCodeIntrinsic(
            Architecture.X86,
            0x55,                          // PUSH EBP
            0x89, 0xe5,                    // MOV EBP,ESP
            0xb8, 0x1f, 0x00, 0x00, 0x00,  // MOV EAX,31
            0x5d,                          // POP EBP
            SupportedOSPlatforms = ["windows", "linux"])]
        [MethodImpl(Intrinsics.MethodImplOptions)]
        public static int FrameChainIntrinsic() => OperationResults.Managed;
    }

    public static class PathologicalUnwind
    {
        static PathologicalUnwind()
        {
            Intrinsics.InitializeType(typeof(PathologicalUnwind));
        }

        [MachineCodeIntrinsic(
            Architecture.Arm64,
            0x1f, 0x20, 0x03, 0xd5,  // NOP
            0xfd, 0x7b, 0xbf, 0xa9,  // STP X29,LR,[SP,#-16]!
            0xfd, 0x03, 0x00, 0x91,  // MOV X29,SP
            0xe0, 0x03, 0x80, 0x52,  // MOV W0,31
            0xfd, 0x7b, 0xc1, 0xa8,  // LDP X29,LR,[SP],#16
            SupportedOSPlatforms = ["windows", "linux", "macos"])]
        [MethodImpl(Intrinsics.MethodImplOptions)]
        public static int DelayedFrameChainIntrinsic()
        {
            return OperationResults.Managed;
        }

        [MachineCodeIntrinsic(
            Architecture.Arm64,
            0xff, 0x43, 0x00, 0xd1,  // SUB SP,SP,#16
            0xff, 0x43, 0x00, 0x91,  // ADD SP,SP,#16
            0xff, 0x43, 0x00, 0xd1,  // SUB SP,SP,#16
            0xe0, 0x03, 0x80, 0x52,  // MOV W0,31
            0xff, 0x43, 0x00, 0x91,  // ADD SP,SP,#16
            SupportedOSPlatforms = ["windows", "linux", "macos"])]
        [MethodImpl(Intrinsics.MethodImplOptions)]
        public static int InteriorStackManipulationIntrinsic()
        {
            return OperationResults.Managed;
        }
    }
}
