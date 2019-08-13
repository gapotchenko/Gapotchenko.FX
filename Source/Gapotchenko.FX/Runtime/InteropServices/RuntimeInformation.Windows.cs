using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#if !TPF_RUNTIME_INFORMATION

namespace System.Runtime.InteropServices
{
    partial class RuntimeInformation
    {
        static class Windows
        {
            public static Architecture GetProcessArchitecture()
            {
                NativeMethods.GetSystemInfo(out var systemInfo);
                return ConvertArchitecture(systemInfo.wProcessorArchitecture);
            }

            public static Architecture GetOSArchitecture()
            {
                NativeMethods.GetNativeSystemInfo(out var systemInfo);
                return ConvertArchitecture(systemInfo.wProcessorArchitecture);
            }

            static Architecture ConvertArchitecture(NativeMethods.ProcessorArchitecture processorArchitecture)
            {
                switch (processorArchitecture)
                {
                    case NativeMethods.ProcessorArchitecture.ARM64:
                        return Architecture.Arm64;
                    case NativeMethods.ProcessorArchitecture.ARM:
                        return Architecture.Arm;
                    case NativeMethods.ProcessorArchitecture.AMD64:
                        return Architecture.X64;
                    case NativeMethods.ProcessorArchitecture.INTEL:
                        return Architecture.X86;
                    default:
                        throw new NotSupportedException(
                            string.Format(
                                "Processor architecture 0x{0:X} reported by Windows OS is not supported.",
                                processorArchitecture));
                }
            }

            static class NativeMethods
            {
                public enum ProcessorArchitecture : ushort
                {
                    INTEL = 0,
                    ARM = 5,
                    IA64 = 6,
                    AMD64 = 9,
                    ARM64 = 12,
                    UNKNOWN = 0xFFFF
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct SYSTEM_INFO
                {
                    public ProcessorArchitecture wProcessorArchitecture;
                    public ushort wReserved;
                    public int dwPageSize;
                    public IntPtr lpMinimumApplicationAddress;
                    public IntPtr lpMaximumApplicationAddress;
                    public IntPtr dwActiveProcessorMask;
                    public int dwNumberOfProcessors;
                    public int dwProcessorType;
                    public int dwAllocationGranularity;
                    public short wProcessorLevel;
                    public short wProcessorRevision;
                }

                [DllImport("kernel32.dll")]
                public extern static void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

                [DllImport("kernel32.dll")]
                public extern static void GetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);
            }
        }
    }
}

#endif
