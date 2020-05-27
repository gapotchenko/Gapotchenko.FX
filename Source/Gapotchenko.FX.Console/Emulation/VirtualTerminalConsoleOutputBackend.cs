using System;
using System.IO;

namespace Gapotchenko.FX.Console.Emulation
{
    using Console = System.Console;

    sealed class VirtualTerminalConsoleOutputBackend : IVirtualTerminalOutputBackend
    {
        public VirtualTerminalConsoleOutputBackend() :
            this(Console.Out)
        {
        }

        public VirtualTerminalConsoleOutputBackend(TextWriter textWriter)
        {
            if (textWriter == null)
                throw new ArgumentNullException(nameof(textWriter));
            Out = textWriter;
        }

        public TextWriter Out { get; }

        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        public void ResetColor()
        {
            Console.ResetColor();
        }

        public void ResetForegroundColor()
        {
            var backgroundColor = Console.BackgroundColor;
            Console.ResetColor();
            Console.BackgroundColor = backgroundColor;
        }

        public void ResetBackgroundColor()
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ResetColor();
            Console.ForegroundColor = foregroundColor;
        }
    }
}
