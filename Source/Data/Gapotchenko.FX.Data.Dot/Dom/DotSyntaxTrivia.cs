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
    public readonly struct DotSyntaxTrivia
    {
        public DotSyntaxTrivia(DotToken kind, string value)
        {
            Kind = kind;
            Value = value;
        }

        public DotToken Kind { get; }
        public string Value { get; }
    }
}
