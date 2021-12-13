using System.Collections.Generic;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a significant token in the syntax tree.
    /// </summary>
    public abstract class DotSignificantToken : DotElement
    {
        /// <summary>
        /// Token text.
        /// </summary>
        public abstract string Text { get; set; }

        /// <summary>
        /// Token value.
        /// </summary>
        /// <remarks>
        /// For example, if the token represents a string literal, then this property would return the actual string value.
        /// </remarks>
        public abstract string Value { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        List<DotInsignificantToken>? _leadingTrivia;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        List<DotInsignificantToken>? _trailingTrivia;

        /// <inheritdoc/>
        public override IList<DotInsignificantToken> LeadingTrivia => _leadingTrivia ??= new();

        /// <inheritdoc/>
        public override IList<DotInsignificantToken> TrailingTrivia => _trailingTrivia ??= new();

        /// <inheritdoc/>
        public override bool HasLeadingTrivia => _leadingTrivia?.Count > 0;

        /// <inheritdoc/>
        public override bool HasTrailingTrivia => _trailingTrivia?.Count > 0;

        /// <summary>
        /// Returns the string representation of this token.
        /// </summary>
        public override string ToString() => Text;
    }
}
