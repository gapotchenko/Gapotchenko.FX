using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a significant token in the syntax tree.
    /// </summary>
    public abstract class DotSignificantToken : DotToken, IDotElement, IDotSyntaxSlotProvider
    {
        /// <summary>
        /// Initializes a new <see cref="DotSignificantToken"/> instance.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="text"></param>
        protected internal DotSignificantToken(DotTokenKind kind, string text)
            : base(kind, text)
        {
        }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        /// <remarks>
        /// For example, if the token represents a string literal, then this property would return the actual string value.
        /// </remarks>
        public virtual string Value
        {
            get => Text;
            set => Text = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        NotNullList<DotTrivia>? _leadingTrivia;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        NotNullList<DotTrivia>? _trailingTrivia;

        /// <inheritdoc/>
        public IList<DotTrivia> LeadingTrivia => _leadingTrivia ??= new();

        /// <inheritdoc/>
        public IList<DotTrivia> TrailingTrivia => _trailingTrivia ??= new();

        /// <inheritdoc/>
        public bool HasLeadingTrivia => _leadingTrivia?.Count > 0;

        /// <inheritdoc/>
        public bool HasTrailingTrivia => _trailingTrivia?.Count > 0;

        int IDotSyntaxSlotProvider.SlotCount => 0;

        IDotSyntaxSlotProvider IDotSyntaxSlotProvider.GetSlot(int i) => throw new ArgumentOutOfRangeException(nameof(i));
    }
}
