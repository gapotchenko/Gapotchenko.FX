using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a trivia in the syntax tree.
    /// </summary>
    public readonly struct DotTrivia
    {
        /// <summary>
        /// Initializes a new <see cref="DotTrivia"/> instance.
        /// </summary>
        /// <param name="kind">Trivia kind.</param>
        /// <param name="value">Trivia value.</param>
        public DotTrivia(DotTokenKind kind, string value)
        {
            Kind = kind;
            Value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="DotTrivia"/> instance.
        /// </summary>
        /// <param name="value">Trivia value.</param>
        public DotTrivia(char value)
        {
            Kind = (DotTokenKind)value;
            Value = value.ToString();
        }

        /// <summary>
        /// Trivia kind.
        /// </summary>
        public DotTokenKind Kind { get; }

        /// <summary>
        /// Trivia value.
        /// </summary>
        public string Value { get; }
    }
}
