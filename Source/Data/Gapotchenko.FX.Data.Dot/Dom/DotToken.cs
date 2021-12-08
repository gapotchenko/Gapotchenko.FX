using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a token in the syntax tree.
    /// </summary>
    public sealed class DotToken : DotElement
    {
        /// <summary>
        /// Creates a new <see cref="DotTrivia"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="value">Token value.</param>
        public DotToken(DotTokenKind kind, string? value = default)
        {
            if (value is null &&
                !kind.TryGetDefaultValue(out value))
            {
                throw new ArgumentException("Value cannot deducted from the kind.", nameof(value));
            }

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

        /// <inheritdoc/>
        public override IList<DotTrivia> LeadingTrivia => _leadingTrivia ??= new();

        /// <inheritdoc/>
        public override IList<DotTrivia> TrailingTrivia => _trailingTrivia ??= new();

        /// <inheritdoc/>
        public override bool HasLeadingTrivia => _leadingTrivia?.Count > 0;

        /// <inheritdoc/>
        public override bool HasTrailingTrivia => _trailingTrivia?.Count > 0;

        static readonly Regex ValidIdentifierPattern = new(
            @"^(([a-zA-Z\200-\377_][0-9a-zA-Z\200-\377_]*)|(-?(\.[0-9]+|[0-9]+(\.[0-9]*)?)))$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        static string EscapeIdentifier(string identifier) =>
            identifier
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r\n", "\\r\\n")
                .Replace("\n", "\\n");

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
