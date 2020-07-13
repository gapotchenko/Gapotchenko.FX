using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#nullable enable

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
