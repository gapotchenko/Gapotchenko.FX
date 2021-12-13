using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a significant token in the syntax tree.
    /// </summary>
    public abstract class DotSignificantToken : DotElement, IDotSyntaxSlotProvider
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
        public IList<DotInsignificantToken> LeadingTrivia => _leadingTrivia ??= new();

        /// <inheritdoc/>
        public IList<DotInsignificantToken> TrailingTrivia => _trailingTrivia ??= new();

        /// <inheritdoc/>
        public bool HasLeadingTrivia => _leadingTrivia?.Count > 0;

        /// <inheritdoc/>
        public bool HasTrailingTrivia => _trailingTrivia?.Count > 0;

        /// <summary>
        /// Returns the string representation of this token.
        /// </summary>
        public override string ToString() => Text;

        int IDotSyntaxSlotProvider.SlotCount => 0;
        IDotSyntaxSlotProvider IDotSyntaxSlotProvider.GetSlot(int i) => throw new ArgumentOutOfRangeException(nameof(i));
    }
}
