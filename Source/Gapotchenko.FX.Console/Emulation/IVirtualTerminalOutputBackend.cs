using System;
using System.IO;

namespace Gapotchenko.FX.Console.Emulation
{
    interface IVirtualTerminalOutputBackend
    {
        TextWriter Out { get; }
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }

        void ResetColor();
        void ResetForegroundColor();
        void ResetBackgroundColor();
    }
}
