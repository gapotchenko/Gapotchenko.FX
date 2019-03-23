// Portions of the code came from Boost Program Options library authored by Vladimir Prus.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Diagnostics
{
    partial class CommandLine
    {
        /// <summary>
        /// Splits a command line into arguments.
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
        /// Splits a specified command line into arguments.
        /// </summary>
        /// <param name="commandLine">Command line to split.</param>
        /// <returns>A sequence of command line arguments.</returns>
        public static IEnumerable<string> Split(string commandLine) =>
            Split(new StringReader(commandLine ?? throw new ArgumentNullException(nameof(commandLine))));
    }
}
