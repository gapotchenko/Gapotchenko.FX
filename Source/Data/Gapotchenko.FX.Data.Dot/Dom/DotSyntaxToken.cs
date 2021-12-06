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
    public sealed class DotSyntaxToken
    {
        /// <summary>
        /// Creates a new <see cref="DotSyntaxTrivia"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="value">Token value.</param>
        public DotSyntaxToken(DotToken kind, string value)
        {
            Kind = kind;
            Value = value;
        }

        /// <summary>
        /// Token kind.
        /// </summary>
        public DotToken Kind { get; }

        /// <summary>
        /// Token value.
        /// </summary>
        public string Value { get; }

        List<DotSyntaxTrivia>? _leadingTrivia;
        List<DotSyntaxTrivia>? _trailingTrivia;

        /// <summary>
        /// The list of trivia that appear before this token.
        /// </summary>
        public List<DotSyntaxTrivia> LeadingTrivia =>
            _leadingTrivia ??= new();

        /// <summary>
        /// The list of trivia that appear after this token.
        /// </summary>
        public List<DotSyntaxTrivia> TrailingTrivia =>
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
