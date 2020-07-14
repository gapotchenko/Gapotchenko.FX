using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable enable

namespace Gapotchenko.FX.Diagnostics.Implementation.Windows
{
    static class SystemInfo
    {
        static SystemInfo()
        {
            NativeMethods.GetSystemInfo(out var systemInfo);

            int error = Marshal.GetLastWin32Error();
            if (error != 0)
                throw new Win32Exception(error);

            PageSize = checked((int)systemInfo.dwPageSize);
        }

        public static int PageSize { get; }

        public static class Native
        {
            static Native()
            {
                NativeMethods.GetNativeSystemInfo(out var systemInfo);

                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                    throw new Win32Exception(error);

                PageSize = checked((int)systemInfo.dwPageSize);
            }

            public static int PageSize { get; }
        }
    }
}
