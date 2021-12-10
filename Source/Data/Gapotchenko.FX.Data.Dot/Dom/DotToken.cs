using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a token in the syntax tree.
    /// </summary>
    public sealed class DotToken : DotElement
    {
        /// <summary>
        /// Creates a new <see cref="DotToken"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        public DotToken(DotTokenKind kind, string? text = default)
        {
            if (text is null &&
                !kind.TryGetDefaultValue(out text))
            {
                throw new ArgumentException("Token text cannot deducted from the kind.", nameof(text));
            }

            Kind = kind;
            Text = text;
        }

        /// <summary>
        /// Token kind.
        /// </summary>
        public DotTokenKind Kind { get; }

        /// <summary>
        /// Token text.
        /// </summary>
        public string Text { get; }

        string? _value;
        bool _valueInitialized;

        /// <summary>
        /// Token value.
        /// </summary>
        /// <remarks>
        /// For example, if the token represents a string literal, then this property would return the actual string value.
        /// </remarks>
        public string? Value
        {
            get
            {
                if (!_valueInitialized)
                {
                    _value = ComputeValue(Kind, Text);
                    _valueInitialized = true;
                }

                return _value;
            }
        }

        List<DotTrivia>? _leadingTrivia;
        List<DotTrivia>? _trailingTrivia;

        /// <inheritdoc/>
        public override IList<DotTrivia> LeadingTrivia => _leadingTrivia ??= new();

        /// <inheritdoc/>
        public override IList<DotTrivia> TrailingTrivia => _trailingTrivia ??= new();

        /// <inheritdoc/>
        public override bool HasLeadingTrivia => _leadingTrivia?.Count > 0;

        /// <inheritdoc/>
        public override bool HasTrailingTrivia => _trailingTrivia?.Count > 0;

        static string? ComputeValue(DotTokenKind kind, string text) =>
            kind switch
            {
                DotTokenKind.Id => UnescapeIdentifier(text),
                _ => null,
            };

        [return: NotNullIfNotNull("identifier")]
        static string? UnescapeIdentifier(string? identifier, bool strict = true)
        {
            if (identifier is null)
                return null;

            if (identifier.IndexOf('\\') is -1)
                return identifier;

            var sb = new StringBuilder();

            for (int i = 0; i < identifier.Length; i++)
            {
                var ch = identifier[i];

                if (ch is '\\' && i < identifier.Length - 1)
                {
                    var escapedChar = identifier[++i];
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
                                throw new FormatException($"Cannot expand escape sequence \"\\{escapedChar}\" of identifier \"{identifier}\".");
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
                            if (i < identifier.Length - 1 && identifier[i + 1] is '\n')
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

        [return: NotNullIfNotNull("value")]
        static string? EscapeIdentifier(string? identifier)
        {
            if (identifier is null)
                return null;

            return identifier
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r\n", "\\r\\n")
                .Replace("\n", "\\n");
        }

        /// <summary>
        /// Escapes the given identifier and wraps it into a new token.
        /// </summary>
        /// <param name="identifier">Identifier value.</param>
        public static DotToken CreateIdentifierToken(string? identifier)
        {
            identifier ??= string.Empty;
            identifier = EscapeIdentifier(identifier);

            var token = new DotToken(DotTokenKind.Id, identifier);

            if (string.IsNullOrEmpty(identifier) ||
                !ValidIdentifierPattern.IsMatch(identifier))
            {
                token.LeadingTrivia.Add(new DotTrivia(DotTokenKind.Quote));
                token.TrailingTrivia.Add(new DotTrivia(DotTokenKind.Quote));
            }

            return token;
        }
    }
}
