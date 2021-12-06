using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public DotToken(DotTokenKind kind, string? value)
        {
            if (value is null)
            {
                value = kind switch
                {
                    DotTokenKind.Digraph => "digraph",
                    DotTokenKind.Graph => "graph",
                    DotTokenKind.Arrow => "->",
                    _ => throw new ArgumentException("Value cannot deducted from the kind.", nameof(value))
                };
            }

            Kind = kind;
            Value = value;
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
        public List<DotTrivia> LeadingTrivia =>
            _leadingTrivia ??= new();

        /// <summary>
        /// The list of trivia that appear after this token.
        /// </summary>
        public List<DotTrivia> TrailingTrivia =>
            _trailingTrivia ??= new();

        /// <summary>
        /// Determines whether this token has any leading trivia.
        /// </summary>
        public bool HasLeadingTrivia => _leadingTrivia?.Count > 0;

        /// <summary>
        /// Determines whether this token has any trailing trivia.
        /// </summary>
        public bool HasTrailingTrivia => _trailingTrivia?.Count > 0;
    }
}
