// Portions of code come from MSBuild project from Microsoft.

using Gapotchenko.FX.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Diagnostics
{
    /// <summary>
    /// Represents a mutable string of command line parameters.
    /// </summary>
    public sealed class CommandLineBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineBuilder"/> class.
        /// </summary>
        public CommandLineBuilder() :
            this(false)
        {
        }

        CommandLineBuilder(bool quoteHyphensOnCommandLine)
        {
            _CommandLine = new StringBuilder();
            _QuoteHyphens = quoteHyphensOnCommandLine;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        StringBuilder _CommandLine;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool _QuoteHyphens;

        /// <summary>
        /// Appends a specified command line argument to this instance.
        /// The argument text is automatically quoted according to the command line rules.
        /// </summary>
        /// <param name="argument">The command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendArgument(string argument)
        {
            if (argument != null)
            {
                _SeparateArguments();
                _AppendTextWithQuoting(argument);
            }
            return this;
        }

        /// <summary>
        /// Appends a specified command line argument that represents a file name to this instance.
        /// The file name is automatically quoted according to the command line rules.
        /// </summary>
        /// <param name="fileName">The command line argument that represents a file name to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendFileName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && fileName[0] == '-')
                return AppendArgument(@".\" + fileName);
            else
                return AppendArgument(fileName);
        }

        /// <summary>
        /// Gets the raw string builder for the command line.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public StringBuilder Raw => _CommandLine;

        /// <summary>
        /// <para>
        /// Separates command line arguments by ensuring that either the last appended character is a whitespace (<see cref="CommandLine.ArgumentsSeparator"/>) or the built command line is empty.
        /// </para>
        /// <para>
        /// This method is used in conjunction with <see cref="Raw"/> property.
        /// </para>
        /// </summary>
        /// <returns>The instance of command line builder.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public CommandLineBuilder SeparateArguments()
        {
            _SeparateArguments();
            return this;
        }

        void _SeparateArguments()
        {
            if (_CommandLine.Length != 0 && _CommandLine[_CommandLine.Length - 1] != CommandLine.ArgumentsSeparator)
                _CommandLine.Append(CommandLine.ArgumentsSeparator);
        }

        void _AppendQuotedTextToBuffer(StringBuilder buffer, string unquotedTextToAppend)
        {
            if (unquotedTextToAppend == null)
                return;

            bool quotingRequired = _IsQuotingRequired(unquotedTextToAppend);
            if (quotingRequired)
                buffer.Append('"');

            int numberOfQuotes = 0;
            for (int i = 0; i < unquotedTextToAppend.Length; i++)
            {
                if (unquotedTextToAppend[i] == '"')
                    numberOfQuotes++;
            }

            if (numberOfQuotes > 0)
            {
                if ((numberOfQuotes % 2) != 0)
                    throw new Exception("Command line parameter cannot contain odd number of double quotes.");
                unquotedTextToAppend = unquotedTextToAppend.Replace("\\\"", "\\\\\"").Replace("\"", "\\\"");
            }

            buffer.Append(unquotedTextToAppend);

            if (quotingRequired && unquotedTextToAppend.EndsWith('\\'))
                buffer.Append('\\');

            if (quotingRequired)
                buffer.Append('"');
        }

        void _AppendTextWithQuoting(string textToAppend)
        {
            _AppendQuotedTextToBuffer(_CommandLine, textToAppend);
        }

        bool _IsQuotingRequired(string parameter)
        {
            if (parameter == null)
                return false;

            if (!_AllowedUnquotedRegex.IsMatch(parameter))
                return true;

            if (_DefinitelyNeedQuotesRegex.IsMatch(parameter))
                return true;

            return false;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Regex _CachedAllowedUnquotedRegex;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Regex _AllowedUnquotedRegex
        {
            get
            {
                if (_CachedAllowedUnquotedRegex == null)
                {
                    if (_QuoteHyphens)
                        _CachedAllowedUnquotedRegex = new Regex(@"^[a-z\\/:0-9\._+=]*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                    else
                        _CachedAllowedUnquotedRegex = new Regex(@"^[a-z\\/:0-9\._\-+=]*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                }
                return _CachedAllowedUnquotedRegex;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Regex _CachedDefinitelyNeedQuotesRegex;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Regex _DefinitelyNeedQuotesRegex
        {
            get
            {
                if (_CachedDefinitelyNeedQuotesRegex == null)
                {
                    if (_QuoteHyphens)
                        _CachedDefinitelyNeedQuotesRegex = new Regex("[|><\\s,;\\-\"]+", RegexOptions.CultureInvariant);
                    else
                        _CachedDefinitelyNeedQuotesRegex = new Regex("[|><\\s,;\"]+", RegexOptions.CultureInvariant);
                }
                return _CachedDefinitelyNeedQuotesRegex;
            }
        }

        /// <summary>
        /// Converts the value of this instance to a <see cref="System.String"/>.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public override string ToString() => _CommandLine.ToString();
    }
}
