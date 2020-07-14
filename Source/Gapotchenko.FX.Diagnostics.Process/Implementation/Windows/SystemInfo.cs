#nullable enable

namespace Gapotchenko.FX.Diagnostics.Implementation.Windows
{
    static class SystemInfo
    {
        static SystemInfo()
        {
            NativeMethods.GetSystemInfo(out var systemInfo);
            PageSize = checked((int)systemInfo.dwPageSize);
        }

        public static int PageSize { get; }

        public static class Native
        {
            static Native()
            {
                NativeMethods.GetNativeSystemInfo(out var systemInfo);
                PageSize = checked((int)systemInfo.dwPageSize);
            }

            public static int PageSize { get; }
        }
    }
}
