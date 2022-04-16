using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics.Pal.MacOS
{
#if NET
    [SupportedOSPlatform("macos")]
#endif
    sealed class PalAdapter : IPalAdapter
    {
        PalAdapter()
        {
        }

        public static PalAdapter Instance { get; } = new PalAdapter();

        public unsafe int GetParentProcessId(Process process)
        {
            if (!Environment.Is64BitProcess)
                throw new PlatformNotSupportedException();

            const int infoSize = 648; // sizeof(kinfo_proc)
            var info = stackalloc byte[infoSize]; // kinfo_proc
            var infoLength = new IntPtr(infoSize);

            var mib = new int[] { NativeMethods.CTL_KERN, NativeMethods.KERN_PROC, NativeMethods.KERN_PROC_PID, process.Id };
            if (NativeMethods.sysctl(mib, mib.Length, info, &infoLength, null, IntPtr.Zero) < 0)
                throw new Exception("sysctl failed.");
            if (infoLength == IntPtr.Zero)
                throw new Exception("sysctl returned an unexpected length.");

            return *(int*)(info + 560); // info.kp_eproc.e_ppid
        }

        public string? GetProcessImageFileName(Process process) => null;

        public IReadOnlyDictionary<string, string> ReadProcessEnvironmentVariables(Process process)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryInterruptProcessAsync(Process process, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

