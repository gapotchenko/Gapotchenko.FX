// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Utils;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Encodes Windows x64 unwind information for intrinsic prologues.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
static class UnwindWindowsX64
{
    public static UnwindX64.Analysis Analyze(
        ReadOnlySpan<byte> code,
        Span<UnwindX64.UnwindOperation> operations)
    {
        var analysis = UnwindX64.Analyze(code, operations);
        if (analysis.Result != UnwindAnalysisResult.Supported)
            return analysis;

        return IsSupported(analysis.Operations, analysis.Info) ?
            analysis :
            analysis with { Result = UnwindAnalysisResult.Unsupported };
    }

    public static int GetSize(in UnwindX64.Analysis analysis)
    {
        return
            HeaderSize +
            UnwindCodeSize * AlignUnwindCodeCount(CountUnwindCodes(analysis.Operations));
    }

    public static void Write(Span<byte> destination, in UnwindX64.Analysis unwindAnalysis)
    {
        var unwindOperations = unwindAnalysis.Operations;
        var unwindInfo = unwindAnalysis.Info;
        int unwindCodeCount = CountUnwindCodes(unwindOperations);
        int size = HeaderSize + UnwindCodeSize * AlignUnwindCodeCount(unwindCodeCount);
        destination = destination[..size];
        destination.Clear();

        destination[0] = Version;
        destination[1] = checked((byte)unwindInfo.PrologueSize);
        destination[2] = checked((byte)unwindCodeCount);
        destination[3] = checked((byte)(unwindInfo.FrameOffset / FrameOffsetUnit << 4 | unwindInfo.FrameRegister));

        int destinationOffset = HeaderSize;
        for (int i = unwindOperations.Length - 1; i >= 0; --i)
        {
            ref readonly var operation = ref unwindOperations[i];
            GetEncoding(
                operation,
                out var operationCode,
                out int operationInfo,
                out uint extraData,
                out int extraSlotCount);

            destination[destinationOffset] = operation.CodeOffset;
            destination[destinationOffset + 1] = checked((byte)(operationInfo << 4 | (byte)operationCode));
            destinationOffset += UnwindCodeSize;

            switch (extraSlotCount)
            {
                case 1:
                    WriteUInt16(destination[destinationOffset..], checked((ushort)extraData));
                    destinationOffset += UnwindCodeSize;
                    break;

                case 2:
                    WriteUInt32(destination[destinationOffset..], extraData);
                    destinationOffset += 2 * UnwindCodeSize;
                    break;
            }
        }
    }

    static bool IsSupported(ReadOnlySpan<UnwindX64.UnwindOperation> operations, UnwindX64.UnwindInfo info)
    {
        if (info.FrameRegister != 0 &&
            (info.FrameOffset % FrameOffsetUnit != 0 || info.FrameOffset > MaximumFrameOffset))
        {
            return false;
        }

        return CountUnwindCodes(operations) <= byte.MaxValue;
    }

    static int CountUnwindCodes(ReadOnlySpan<UnwindX64.UnwindOperation> operations)
    {
        int count = 0;
        foreach (ref readonly var operation in operations)
        {
            GetEncoding(operation, out _, out _, out _, out int extraSlotCount);
            count = checked(count + 1 + extraSlotCount);
        }
        return count;
    }

    static void GetEncoding(
        UnwindX64.UnwindOperation operation,
        out NativeMethods.UnwindOperation operationCode,
        out int operationInfo,
        out uint extraData,
        out int extraSlotCount)
    {
        operationInfo = 0;
        extraData = 0;
        extraSlotCount = 0;

        switch (operation.Kind)
        {
            case UnwindX64.UnwindOperationKind.PushNonvolatile:
                operationCode = NativeMethods.UnwindOperation.PushNonvolatile;
                operationInfo = operation.Register;
                break;

            case UnwindX64.UnwindOperationKind.StackAllocation:
                uint allocationSize = operation.Value;
                if (allocationSize <= 128)
                {
                    operationCode = NativeMethods.UnwindOperation.AllocateSmall;
                    operationInfo = checked((int)(allocationSize / 8 - 1));
                }
                else if (allocationSize / 8 <= ushort.MaxValue)
                {
                    operationCode = NativeMethods.UnwindOperation.AllocateLarge;
                    extraData = allocationSize / 8;
                    extraSlotCount = 1;
                }
                else
                {
                    operationCode = NativeMethods.UnwindOperation.AllocateLarge;
                    operationInfo = 1;
                    extraData = allocationSize;
                    extraSlotCount = 2;
                }
                break;

            case UnwindX64.UnwindOperationKind.SetFramePointer:
                operationCode = NativeMethods.UnwindOperation.SetFramePointer;
                break;

            default:
                throw new InvalidOperationException("Unknown x64 unwind operation.");
        }
    }

    static int AlignUnwindCodeCount(int count) => MemoryArithmetics.Align2(count);

    static void WriteUInt16(Span<byte> destination, ushort value)
    {
        destination[0] = (byte)value;
        destination[1] = (byte)(value >> 8);
    }

    static void WriteUInt32(Span<byte> destination, uint value)
    {
        WriteUInt16(destination, (ushort)value);
        WriteUInt16(destination[2..], (ushort)(value >> 16));
    }

    const byte Version = 1;
    const int HeaderSize = 4;
    const int UnwindCodeSize = 2;
    const int FrameOffsetUnit = 16;
    const int MaximumFrameOffset = 240;
}
