// Portions of the code came from MSBuild project authored by Microsoft.

using Gapotchenko.FX.Text;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Diagnostics;

partial class CommandLine
{
    /// <summary>
    /// Escapes and optionally quotes a command line argument.
    /// </summary>
    /// <param name="value">The command line argument.</param>
    /// <returns>The escaped and optionally quoted command line argument.</returns>
    [return: NotNullIfNotNull("value")]
    public static string? EscapeArgument(string? value)
    {
        if (value == null)
            return null;

        int length = value.Length;
        if (length == 0)
            return string.Empty;

        var sb = new StringBuilder();
        Escape.AppendQuotedText(sb, value);

        if (sb.Length == length)
            return value;

        return sb.ToString();
    }

    /// <summary>
    /// Escapes and optionally quotes a command line argument that represents a file name.
    /// </summary>
    /// <param name="value">The command line argument that represents a file name.</param>
    /// <returns>The escaped and optionally quoted command line argument.</returns>
    [return: NotNullIfNotNull("value")]
    public static string? EscapeFileName(string? value) => EscapeArgument(Escape.EncodeFileName(value));

    internal static class Escape
    {
        [return: NotNullIfNotNull("value")]
        public static string? EncodeFileName(string? value)
        {
            if (value == null)
                return null;

            if (value.Length != 0 && value[0] == '-')
                return "." + Path.DirectorySeparatorChar + value;
            else
                return value;
        }

        public static void AppendQuotedText(StringBuilder sb, string text)
        {
            bool quotingRequired = IsQuotingRequired(text);
            if (quotingRequired)
                sb.Append('"');

            int numberOfQuotes = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '"')
                    numberOfQuotes++;
            }

            if (numberOfQuotes > 0)
            {
                if ((numberOfQuotes % 2) != 0)
                    throw new Exception("Command line parameter cannot contain an odd number of double quotes.");
                text = text.Replace("\\\"", "\\\\\"").Replace("\"", "\\\"");
            }

            sb.Append(text);

            if (quotingRequired && text.EndsWith('\\'))
                sb.Append('\\');

            if (quotingRequired)
                sb.Append('"');
        }

        static bool IsQuotingRequired(string parameter) =>
            !AllowedUnquotedRegex.IsMatch(parameter) ||
            DefinitelyNeedQuotesRegex.IsMatch(parameter);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Regex? m_CachedAllowedUnquotedRegex;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Regex AllowedUnquotedRegex =>
            m_CachedAllowedUnquotedRegex ??= new Regex(
                @"^[a-z\\/:0-9\._\-+=]*$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Regex? m_CachedDefinitelyNeedQuotesRegex;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Regex DefinitelyNeedQuotesRegex =>
            m_CachedDefinitelyNeedQuotesRegex ??= new Regex(
                "[|><\\s,;\"]+",
                RegexOptions.CultureInvariant);
    }
}
