// Portions of the code came from MSBuild project authored by Microsoft.

using Gapotchenko.FX.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Diagnostics;

partial class CommandLine
{
    /// <summary>
    /// Escapes and optionally quotes a command-line argument.
    /// </summary>
    /// <param name="value">The command-line argument.</param>
    /// <returns>The escaped and optionally quoted command-line argument.</returns>
    [return: NotNullIfNotNull(nameof(value))]
    public static string? EscapeArgument(string? value)
    {
        if (value is null)
            return null;

        int length = value.Length;
        if (length == 0)
            return string.Empty;

        var sb = new StringBuilder(length + 8);
        Escape.AppendQuotedText(sb, value);

        if (sb.Length == length)
            return value;
        else
            return sb.ToString();
    }

    /// <summary>
    /// Encodes the specified file name so it is safely interpreted as a command-line argument,
    /// rather than being mistaken for an option.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some command-line applications treat arguments beginning with a hyphen (<c>-</c>) as options or flags.
    /// If a file name starts with such a reserved character, it may be incorrectly parsed as an option.
    /// </para>
    /// <para>
    /// This method ensures the file name is treated as a positional argument by prepending <c>./</c> when necessary
    /// (for example, <c>-file.txt</c> becomes <c>./-file.txt</c>).
    /// </para>
    /// </remarks>
    /// <param name="value">The file name to encode.</param>
    /// <returns>The encoded command-line argument.</returns>
    [return: NotNullIfNotNull(nameof(value))]
    public static string? EncodeFileName(string? value) => Escape.EncodeFileName(value);

    internal static class Escape
    {
        [return: NotNullIfNotNull(nameof(value))]
        public static string? EncodeFileName(string? value)
        {
            if (value is null)
                return null;
            else if (value.StartsWith('-'))
                return "." + Path.DirectorySeparatorChar + value;
            else
                return value;
        }

        public static void AppendQuotedText(
            StringBuilder builder,
            string text,
            [CallerArgumentExpression(nameof(text))] string? textArgumentName = null)
        {
            bool quotingRequired = IsQuotingRequired(text);
            if (quotingRequired)
                builder.Append('"');

            int numberOfQuotes = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '"')
                    ++numberOfQuotes;
            }

            if (numberOfQuotes > 0)
            {
                if ((numberOfQuotes % 2) != 0)
                    throw new ArgumentException("Command-line parameter cannot contain an odd number of double quotes.", textArgumentName);

                text = text
                    .Replace("\\\"", "\\\\\"")
                    .Replace("\"", "\\\"");
            }

            builder.Append(text);

            if (quotingRequired && text.EndsWith('\\'))
                builder.Append('\\');

            if (quotingRequired)
                builder.Append('"');
        }

        static bool IsQuotingRequired(string parameter) =>
            !AllowedUnquotedRegex.IsMatch(parameter) ||
            DefinitelyNeedQuotesRegex.IsMatch(parameter);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Regex AllowedUnquotedRegex => m_CachedAllowedUnquotedRegex ??= new(
            @"^[a-z\\/:0-9\._\-+=]*$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Regex? m_CachedAllowedUnquotedRegex;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Regex DefinitelyNeedQuotesRegex => m_CachedDefinitelyNeedQuotesRegex ??= new(
            "[|><\\s,;\"]+",
            RegexOptions.CultureInvariant);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Regex? m_CachedDefinitelyNeedQuotesRegex;
    }
}
