using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Diagnostics.Implementation
{
    static class ImplementationServices
    {
        static bool ProcessesAreEquivalent(Process? a, Process? b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null || b is null)
                return false;

            return
                a.Id == b.Id &&
                a.StartTime == b.StartTime;
        }

        public static bool IsCurrentProcess(Process? process) => ProcessesAreEquivalent(process, Process.GetCurrentProcess());

        public static bool IsValidParentProcess(Process parentProcess, Process childProcess)
        {
            if (parentProcess.StartTime > childProcess.StartTime)
            {
                // The parent process was started after the child process.
                // This condition indicates that the real parent process has exited before, and
                // its process ID has been reused by another unrelated process.
                return false;
            }

            return true;
        }

        static class AdapterFactory
        {
            static AdapterFactory()
            {
                Instance = CreateInstance();
            }

            public static IImplementationAdapter? Instance { get; }

            static IImplementationAdapter? CreateInstance()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return Windows.ImplementationAdapter.Instance;
                else
                    return null;
            }
        }

        public static IImplementationAdapter? AdapterOrDefault => AdapterFactory.Instance;

        public static IImplementationAdapter Adapter => AdapterOrDefault ?? throw new PlatformNotSupportedException();

    }
}
