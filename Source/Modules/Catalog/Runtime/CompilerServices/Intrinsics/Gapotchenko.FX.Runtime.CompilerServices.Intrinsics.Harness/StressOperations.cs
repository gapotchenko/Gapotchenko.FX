// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

static class StressOperations
{
    static StressOperations()
    {
        Intrinsics.InitializeType(typeof(StressOperations));
    }

    // The loops deliberately keep execution inside a stack-modifying trampoline long enough
    // for concurrent garbage collections to suspend and unwind its thread.
    [MachineCodeIntrinsic(
        Architecture.X64,
        0x55,                          // PUSH RBP
        0x48, 0x89, 0xe5,              // MOV RBP,RSP
        0x48, 0x83, 0xec, 0x20,        // SUB RSP,32
        0xb8, 0x1f, 0x00, 0x00, 0x00,  // MOV EAX,31
        0xb9, 0x00, 0x00, 0x10, 0x00,  // MOV ECX,100000h
        0xff, 0xc9,                    // DEC ECX
        0x75, 0xfc,                    // JNZ -4
        0x48, 0x83, 0xc4, 0x20,        // ADD RSP,32
        0x5d,                          // POP RBP
        SupportedOSPlatforms = ["windows", "linux", "macos"])]
    [MachineCodeIntrinsic(
        Architecture.X86,
        0x55,                          // PUSH EBP
        0x89, 0xe5,                    // MOV EBP,ESP
        0x83, 0xec, 0x10,              // SUB ESP,16
        0xb8, 0x1f, 0x00, 0x00, 0x00,  // MOV EAX,31
        0xb9, 0x00, 0x00, 0x10, 0x00,  // MOV ECX,100000h
        0x49,                          // DEC ECX
        0x75, 0xfd,                    // JNZ -3
        0x83, 0xc4, 0x10,              // ADD ESP,16
        0x5d,                          // POP EBP
        SupportedOSPlatforms = ["windows", "linux"])]
    [MachineCodeIntrinsic(
        Architecture.Arm64,
        0xfd, 0x7b, 0xbf, 0xa9,  // STP X29,LR,[SP,#-16]!
        0xfd, 0x03, 0x00, 0x91,  // MOV X29,SP
        0xe0, 0x03, 0x80, 0x52,  // MOV W0,31
        0xe1, 0xff, 0x9f, 0x52,  // MOV W1,65535
        0x21, 0x04, 0x00, 0x71,  // SUBS W1,W1,#1
        0xe1, 0xff, 0xff, 0x54,  // B.NE -4
        0xfd, 0x7b, 0xc1, 0xa8,  // LDP X29,LR,[SP],#16
        SupportedOSPlatforms = ["windows", "linux", "macos"])]
    [MachineCodeIntrinsic(
        Architecture.Arm,
        0x80, 0xb5,  // PUSH {R7,LR}
        0x6f, 0x46,  // MOV R7,SP
        0x84, 0xb0,  // SUB SP,#16
        0x1f, 0x20,  // MOVS R0,31
        0xff, 0x21,  // MOVS R1,255
        0xff, 0x22,  // MOVS R2,255
        0x01, 0x3a,  // SUBS R2,#1
        0xfd, 0xd1,  // BNE -6
        0x01, 0x39,  // SUBS R1,#1
        0xfa, 0xd1,  // BNE -12
        0x04, 0xb0,  // ADD SP,#16
        0x80, 0xbd,  // POP {R7,PC}
        SupportedOSPlatforms = ["linux"])]
    [MethodImpl(Intrinsics.MethodImplOptions)]
    public static int UnwindIntrinsic() => OperationResults.Managed;
}
