// Portions of code come from Boost Program Options library, courtesy of Vladimir Prus.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics
{
    /// <summary>
    /// Performs operations on <see cref="System.String"/> instances that contain command line information.
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Represents the command line arguments separator char.
        /// </summary>
        public const char ArgumentsSeparator = ' ';

        /// <summary>
        /// Gets the maximum length of the command line that can be passed to a process.
        /// </summary>
        public static int MaxLengthForProcess
        {
            get
            {
                // The maximum command line length for the CreateProcess function is 32767 characters. This limitation comes from the UNICODE_STRING structure.
                // https://blogs.msdn.microsoft.com/oldnewthing/20031210-00/?p=41553
                return 32767;
            }
        }

        /// <summary>
        /// Gets the maximum length of the command line that can be used at the prompt.
        /// </summary>
        public static int MaxLengthForPrompt
        {
            get
            {
                // If you are using the CMD.EXE command processor, then you are also subject to the 8191 character command line length limit imposed by CMD.EXE.
                return 8191;
            }
        }

        /// <summary>
        /// Gets the maximum length of the command line that can be passed to the shell.
        /// </summary>
        public static int MaxLengthForShell
        {
            get
            {
                // If you are using the ShellExecute/Ex function, then you become subject to the INTERNET_MAX_URL_LENGTH (around 2048) command line length limit imposed by the ShellExecute/Ex functions.
                // If you are running on Windows 95, then the limit is only MAX_PATH.
                return 2047;
            }
        }

        /// <summary>
        /// Splits command line into arguments.
        /// </summary>
        /// <param name="tr">Command line text reader.</param>
        /// <returns>A sequence of command line arguments.</returns>
        public static IEnumerable<string> Split(TextReader tr)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            _SkipWhitespaces(tr);

            var sb = new StringBuilder();
            bool quoted = false;
            int backslashCount = 0;

            for (; ; )
            {
                int ch = tr.Read();
                if (ch == -1)
                    break;
                var c = (char)ch;

                switch (c)
                {
                    // A quote.
                    case '"':
                        sb.Append('\\', backslashCount / 2);
                        if (backslashCount % 2 == 0)
                        {
                            // '"' preceded by even number (n) of backslashes generates
                            // n/2 backslashes and is a quoted block delimiter
                            quoted = !quoted;
                        }
                        else
                        {
                            // '"' preceded by odd number (n) of backslashes generates
                            // (n-1)/2 backslashes and is literal quote.
                            sb.Append('"');
                        }
                        backslashCount = 0;
                        break;

                    // A backslash.
                    case '\\':
                        ++backslashCount;
                        break;

                    // Not a quote nor a backslash.
                    default:
                        // All accumulated backslashes should be added.
                        if (backslashCount != 0)
                        {
                            sb.Append('\\', backslashCount);
                            backslashCount = 0;
                        }

                        if (!quoted && char.IsWhiteSpace(c))
                        {
                            // A space outside the quoted section terminates the current argument.
                            yield return sb.ToString();
                            sb.Clear();

                            _SkipWhitespaces(tr);
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        break;
                }
            }

            // Add the trailing backslashes.
            if (backslashCount != 0)
                sb.Append('\\', backslashCount);

            // Flush the last token.
            if (sb.Length != 0 || quoted)
                yield return sb.ToString();
        }

        static void _SkipWhitespaces(TextReader tr)
        {
            for (; ; )
            {
                int c = tr.Peek();
                if (c == -1)
                    break;
                if (!char.IsWhiteSpace((char)c))
                    break;
                tr.Read();
            }
        }

        /// <summary>
        /// Splits a specified command line into separate arguments.
        /// </summary>
        /// <param name="commandLine">Command line to split.</param>
        /// <returns>A sequence of command line arguments.</returns>
        public static IEnumerable<string> Split(string commandLine) =>
            Split(new StringReader(commandLine ?? throw new ArgumentNullException(nameof(commandLine))));

        /// <summary>
        /// Builds a command line from a single specified argument.
        /// </summary>
        /// <param name="arg1">The argument.</param>
        /// <returns>The command line.</returns>
        public static string Build(string arg1) =>
            new CommandLineBuilder()
                .AppendArgument(arg1)
                .ToString();

        /// <summary>
        /// Builds a command line from two specified arguments.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <returns>The command line.</returns>
        public static string Build(string arg1, string arg2) =>
            new CommandLineBuilder()
                .AppendArgument(arg1)
                .AppendArgument(arg2)
                .ToString();

        /// <summary>
        /// Builds a command line from three specified arguments.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        /// <returns>The command line.</returns>
        public static string Build(string arg1, string arg2, string arg3) =>
            new CommandLineBuilder()
                .AppendArgument(arg1)
                .AppendArgument(arg2)
                .AppendArgument(arg3)
                .ToString();

        /// <summary>
        /// Builds a command line from four specified arguments.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        /// <param name="arg4">The fourth argument.</param>
        /// <returns>The command line.</returns>
        public static string Build(string arg1, string arg2, string arg3, string arg4) =>
            new CommandLineBuilder()
                .AppendArgument(arg1)
                .AppendArgument(arg2)
                .AppendArgument(arg3)
                .AppendArgument(arg4)
                .ToString();

        /// <summary>
        /// Builds a command line from a sequence of arguments.
        /// </summary>
        /// <param name="args">A sequence of arguments.</param>
        /// <returns>The command line.</returns>
        public static string Build(IEnumerable<string> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var clb = new CommandLineBuilder();
            foreach (var arg in args)
                clb.AppendArgument(arg);
            return clb.ToString();
        }

        /// <summary>
        /// Builds a command line from an array of arguments.
        /// </summary>
        /// <param name="args">An array of arguments.</param>
        /// <returns>The command line.</returns>
        public static string Build(params string[] args) => Build((IEnumerable<string>)args);
    }
}
