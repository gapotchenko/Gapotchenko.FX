using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a string literal in DOT document.
    /// </summary>
    public sealed class DotStringLiteral : DotSignificantToken
    {
        DotStringLiteral(string? value, string text)
        {
            _value = value;
            _text = text;
        }

        /// <summary>
        /// Creates a new <see cref="DotStringLiteral"/> instance.
        /// </summary>
        /// <param name="value">String literal value.</param>
        public DotStringLiteral(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            _text = ValueToText(value);
            _value = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _text;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string? _value;

        /// <inheritdoc />
        public override string Text
        {
            get => _text;
            set
            {
                _text = value ?? throw new ArgumentNullException(nameof(value));
                _value = null;
            }
        }

        /// <inheritdoc />
        public override string Value
        {
            get => _value ??= TextToValue(_text);
            set
            {
                _value = value ?? throw new ArgumentNullException(nameof(value));
                _text = ValueToText(_value);
            }
        }

        /// <summary>
        /// Creates a new <see cref="DotStringLiteral"/> instance from its source text representation.
        /// </summary>
        /// <param name="text">String literal source text.</param>
        /// <returns>String literal.</returns>
        public static DotStringLiteral Parse(string text) =>
            new DotStringLiteral(value: null, text);

        static string TextToValue(string text)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            var value = text;
            if (text.Length >= 2)
            {
                switch ((text[0], text[text.Length - 1]))
                {
                    case ('"', '"'):
                    case ('<', '>'):
                        value = value.Substring(1, text.Length - 2);
                        break;
                }
            }

            return UnescapeStringLiteral(value);
        }

        static string ValueToText(string value)
        {
            var text = EscapeStringLiteral(value);

            if (string.IsNullOrEmpty(text) ||
                !ValidIdentifierPattern.IsMatch(text))
            {
                text = '"' + text + '"';
            }

            return text;
        }

        /// <summary>
        /// Unescapes a string literal.
        /// </summary>
        /// <param name="text">Escaped string literal.</param>
        /// <param name="strict">When true, an unescapable sequence leads to exception; otherwise, it resolves to an empty string.</param>
        /// <returns>String literal value.</returns>
        /// <exception cref="FormatException">Cannot expand escape sequence.</exception>
        [return: NotNullIfNotNull("text")]
        public static string? UnescapeStringLiteral(string? text, bool strict = true)
        {
            if (text is null)
                return null;

            if (text.IndexOf('\\') is -1)
                return text;

            var sb = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                var ch = text[i];

                if (ch is '\\' && i < text.Length - 1)
                {
                    var escapedChar = text[++i];
                    switch (escapedChar)
                    {
                        case 'G': // Name of graph or cluster.
                        case 'N': // Name of the node.
                        case 'E': // Name of the edge.
                        case 'T': // Name of the tail node.
                        case 'H': // Name of the head node.
                        case 'L': // Label attribute value.
                        case 'l': // Left-justified newline.
                        case 'r': // Right-justified newline.
                            if (strict)
                                throw new FormatException($"Cannot expand escape sequence \"\\{escapedChar}\" of string \"{text}\".");
                            break;

                        case 'n':
                            sb.Append('\n');
                            break;

                        case '\\':
                            sb.Append('\\');
                            break;

                        case '\n': // Skip the escaped newlines.
                            break;
                        case '\r':
                            if (i < text.Length - 1 && text[i + 1] is '\n')
                                i++;
                            break;

                        default:
                            sb.Append(escapedChar);
                            break;
                    }
                }
                else
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        static readonly Regex ValidIdentifierPattern = new(
            @"^(([a-zA-Z\200-\377_][0-9a-zA-Z\200-\377_]*)|(-?(\.[0-9]+|[0-9]+(\.[0-9]*)?)))$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        /// <summary>
        /// Escapes a string literal.
        /// </summary>
        /// <param name="value">String literal value.</param>
        /// <returns>Escaped string literal value.</returns>
        [return: NotNullIfNotNull("value")]
        public static string? EscapeStringLiteral(string? value)
        {
            if (value is null)
                return null;

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r\n", "\\r\\n")
                .Replace("\n", "\\n");
        }
    }
}
