using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a token in the syntax tree.
    /// </summary>
    public sealed class DotToken
    {
        /// <summary>
        /// Creates a new <see cref="DotTrivia"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="value">Token value.</param>
        public DotToken(DotTokenKind kind, string? value = default)
        {
            value ??= kind switch
            {
                DotTokenKind.Digraph => "digraph",
                DotTokenKind.Graph => "graph",
                DotTokenKind.Arrow => "->",
                < DotTokenKind.EOF => ((char)kind).ToString(),
                _ => throw new ArgumentException("Value cannot deducted from the kind.", nameof(value))
            };

            Kind = kind;
            Value = value;
        }

        /// <summary>
        /// Creates a new <see cref="DotToken"/> instance.
        /// </summary>
        /// <param name="value">Token value.</param>
        public DotToken(char value)
        {
            Kind = (DotTokenKind)value;
            Value = value.ToString();
        }

        /// <summary>
        /// Token kind.
        /// </summary>
        public DotTokenKind Kind { get; }

        /// <summary>
        /// Token value.
        /// </summary>
        public string Value { get; }

        List<DotTrivia>? _leadingTrivia;
        List<DotTrivia>? _trailingTrivia;

        /// <summary>
        /// The list of trivia that appear before this token.
        /// </summary>
        public IList<DotTrivia> LeadingTrivia =>
            _leadingTrivia ??= new();

        /// <summary>
        /// The list of trivia that appear after this token.
        /// </summary>
        public IList<DotTrivia> TrailingTrivia =>
            _trailingTrivia ??= new();

        /// <summary>
        /// Determines whether this token has any leading trivia.
        /// </summary>
        public bool HasLeadingTrivia => _leadingTrivia?.Count > 0;

        /// <summary>
        /// Determines whether this token has any trailing trivia.
        /// </summary>
        public bool HasTrailingTrivia => _trailingTrivia?.Count > 0;

        static readonly Regex ValidIdentifierPattern = new(
            @"^(([a-zA-Z\200-\377_][0-9a-zA-Z\200-\377_]*)|(-?(\.[0-9]+|[0-9]+(\.[0-9]*)?)))$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        static string EscapeIdentifier(string identifier)
        {
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
                token.LeadingTrivia.Add(new DotTrivia('"'));
                token.TrailingTrivia.Add(new DotTrivia('"'));
            }

            return token;
        }
    }
}
