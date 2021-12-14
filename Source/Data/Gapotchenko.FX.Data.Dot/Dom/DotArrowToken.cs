using Gapotchenko.FX.Data.Dot.Serialization;
using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents an arrow token in DOT document.
    /// </summary>
    public sealed class DotArrowToken : DotSignificantToken
    {
        /// <summary>
        /// Initializes a new <see cref="DotArrowToken"/> instance.
        /// </summary>
        /// <param name="arrowKind">Arrow kind.</param>
        public DotArrowToken(DotArrowKind arrowKind)
            : this(ArrowKindToString(arrowKind))
        {
        }

        /// <summary>
        /// Initializes a new <see cref="DotArrowToken"/> instance.
        /// </summary>
        /// <param name="text">Token text.</param>
        public DotArrowToken(string? text = default)
            : base(DotTokenKind.Arrow, text ?? DotTokenKind.Arrow.GetDefaultValue())
        {
        }

        /// <summary>
        /// Arrow kind.
        /// </summary>
        public DotArrowKind ArrowKind
        {
            get => ParseArrowKind(Text);
            set => ArrowKindToString(value);
        }

        static DotArrowKind ParseArrowKind(string text)
        {
            return text switch
            {
                "->" => DotArrowKind.LeftToRight,
                "--" => DotArrowKind.Bidirectional,
                _ => throw new ArgumentOutOfRangeException(nameof(text), $"Unknown arrow kind: {text}.")
            };
        }

        static string ArrowKindToString(DotArrowKind kind) =>
            kind switch
            {
                DotArrowKind.LeftToRight => "->",
                DotArrowKind.Bidirectional => "--",
                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };
    }
}
