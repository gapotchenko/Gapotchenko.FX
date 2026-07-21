using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
static unsafe class NativeMethods
{
#pragma warning disable CS0649 // Field is never assigned to
    public struct RuntimeFunction
    {
        public uint BeginAddress;
        public uint EndAddress;
        public uint UnwindData;
    }
#pragma warning restore CS0649 // Field is never assigned to

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
    public static extern bool FlushInstructionCache(IntPtr hProcess, void* lpBaseAddress, nuint dwSize);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    public static extern RuntimeFunction* RtlLookupFunctionEntry(void* controlPc, out void* imageBase, void* historyTable);
}
