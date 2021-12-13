using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a primitive token in DOT document.
    /// </summary>
    public abstract class DotPrimitiveToken : DotSignificantToken
    {
        /// <summary>
        /// Initializes a new <see cref="DotPrimitiveToken"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        internal protected DotPrimitiveToken(DotTokenKind kind, string? text = default)
        {
            if (text is null)
            {
                if (!kind.TryGetDefaultValue(out text))
                    throw new ArgumentException("Token text cannot deducted from the kind.", nameof(text));
            }

            _text = text;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _text;

        /// <inheritdoc />
        public override string Text
        {
            get => _text;
            set => _text = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public override string Value
        {
            get => Text;
            set => Text = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
