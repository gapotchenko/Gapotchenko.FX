using Gapotchenko.FX.Data.Dot.Serialization;
using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a punctuation token in DOT document.
    /// </summary>
    public sealed class DotPunctuationToken : DotSignificantToken
    {
        /// <summary>
        /// Initializes a new <see cref="DotPunctuationToken"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        public DotPunctuationToken(DotTokenKind kind, string? text = default)
            : base(kind, text ?? kind.GetDefaultValue())
        {
            switch (kind)
            {
                case DotTokenKind.ScopeStart:
                case DotTokenKind.ScopeEnd:
                case DotTokenKind.Semicolon:
                case DotTokenKind.Equal:
                case DotTokenKind.ListStart:
                case DotTokenKind.ListEnd:
                case DotTokenKind.Comma:
                case DotTokenKind.Colon:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }
    }
}
