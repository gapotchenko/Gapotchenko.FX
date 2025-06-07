using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Console;

#if NET
[SupportedOSPlatform("windows")]
#endif
static class NativeMethods
{
    public static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

    public const int STD_OUTPUT_HANDLE = -11;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    [Flags]
    public enum ConsoleOutputMode : uint
    {
        ENABLE_PROCESSED_OUTPUT = 0x0001,
        ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
        ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
        DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
        ENABLE_LVB_GRID_WORLDWIDE = 0x0010,
    }

    [DllImport("kernel32.dll")]
    public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [StructLayout(LayoutKind.Sequential)]
    public struct COORD(short x, short y)
    {
        public short X = x;
        public short Y = y;
    }

    public const int LF_FACESIZE = 32;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CONSOLE_FONT_INFO_EX
    {
        public int cbSize;
        public int nFont;
        public COORD dwFontSize;
        public int FontFamily;
        public int FontWeight;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
        public string FaceName;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool GetCurrentConsoleFontEx(
           IntPtr consoleOutput,
           bool maximumWindow,
           ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

    public const int TMPF_TRUETYPE = 4;
}
