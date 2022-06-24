using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Diagnostics;

partial class CommandLine
{
    static class Limits
    {
        static Limits()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // The maximum command line length for the CreateProcess function is 32767 characters. This limitation comes from the UNICODE_STRING structure.
                // https://blogs.msdn.microsoft.com/oldnewthing/20031210-00/?p=41553
                MaxLengthForProcess = 32767;

                // If you are using the CMD.EXE command processor, then you are also subject to the 8191 character command line length limit imposed by CMD.EXE.
                MaxLengthForPrompt = 8191;

                // If you are using the ShellExecute/Ex function, then you become subject to the INTERNET_MAX_URL_LENGTH (around 2048) command line length limit imposed by the ShellExecute/Ex functions.
                // If you are running on Windows 95, then the limit is only MAX_PATH.
                MaxLengthForShell = 2048;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // TODO: Use Unix limits https://www.in-ulm.de/~mascheck/various/argmax/
                int argmax = 65535;

                MaxLengthForProcess = argmax;
                MaxLengthForPrompt = argmax;
                MaxLengthForShell = argmax;
            }
            else
            {
                // The safe defaults.
                const int argmax = 8191;
                MaxLengthForProcess = argmax;
                MaxLengthForPrompt = argmax;
                MaxLengthForShell = argmax;
            }
        }

        public static readonly int MaxLengthForProcess;
        public static readonly int MaxLengthForPrompt;
        public static readonly int MaxLengthForShell;
    }

    /// <summary>
    /// Gets the maximum length of the command line that can be passed to a process.
    /// </summary>
    public static int MaxLengthForProcess => Limits.MaxLengthForProcess;

    /// <summary>
    /// Gets the maximum length of the command line that can be used at the prompt.
    /// </summary>
    public static int MaxLengthForPrompt => Limits.MaxLengthForPrompt;

    /// <summary>
    /// Gets the maximum length of the command line that can be passed to the shell.
    /// </summary>
    public static int MaxLengthForShell => Limits.MaxLengthForShell;
}
