using System.Runtime.InteropServices;

#pragma warning disable CS0649 // Field is never assigned to

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
static unsafe class NativeMethods
{
    public struct RuntimeFunctionX64
    {
        public uint BeginAddress;
        public uint EndAddress;
        public uint UnwindData;
    }

    public struct RuntimeFunctionArm64
    {
        public uint BeginAddress;
        public uint UnwindData;
    }

    public struct MemoryBasicInformation
    {
        public void* BaseAddress;
        public void* AllocationBase;
        public PageProtect AllocationProtect;
        public nuint RegionSize;
        public PageState State;
        public PageProtect Protect;
        public uint Type;
    }

    public struct MemoryAddressRequirements
    {
        public void* LowestStartingAddress;
        public void* HighestEndingAddress;
        public nuint Alignment;
    }

    public struct MemoryExtendedParameter
    {
        public MemoryExtendedParameterType Type;
        public void* Pointer;
    }

    public enum MemoryExtendedParameterType : ulong
    {
        /// <summary>
        /// Indicates the presence of <see cref="MemoryAddressRequirements"/> structure.
        /// </summary>
        AddressRequirements = 1
    }

    public enum PageState : uint
    {
        MemCommit = 0x1000
    }

    public enum ProcessorFeature : uint
    {
        ArmV8Crc32InstructionsAvailable = 31
    }

    [Flags]
    public enum VirtualAllocationType : uint
    {
        Commit = 0x1000,
        Reserve = 0x2000
    }

    [Flags]
    public enum PageProtect : uint
    {
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        Guard = 0x100,
        NoCache = 0x200,
        WriteCombine = 0x400
    }

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    public static extern bool VirtualProtect(void* lpAddress, nuint dwSize, PageProtect flNewProtect, out PageProtect lpflOldProtect);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    public static extern void* VirtualAlloc(void* lpAddress, nuint dwSize, VirtualAllocationType flAllocationType, PageProtect flProtect);

    [DllImport("KernelBase.dll", SetLastError = true, ExactSpelling = true)]
    public static extern void* VirtualAlloc2(
        IntPtr process,
        void* baseAddress,
        nuint size,
        VirtualAllocationType allocationType,
        PageProtect pageProtection,
        MemoryExtendedParameter* extendedParameters,
        uint parameterCount);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    public static extern nuint VirtualQuery(void* lpAddress, out MemoryBasicInformation lpBuffer, nuint dwLength);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    public static extern bool FlushInstructionCache(IntPtr hProcess, void* lpBaseAddress, nuint dwSize);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    public static extern bool IsProcessorFeaturePresent(ProcessorFeature processorFeature);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    public static extern void* RtlLookupFunctionEntry(void* controlPc, out void* imageBase, void* historyTable);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool RtlAddFunctionTable(RuntimeFunctionX64* functionTable, uint entryCount, nuint baseAddress);
}
