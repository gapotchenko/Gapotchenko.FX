using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Serialization
{
    static class DotTokenKindExtensions
    {
        public static bool TryGetDefaultValue(this DotTokenKind token, [NotNullWhen(true)] out string? value)
        {
            value = token switch
            {
                DotTokenKind.Digraph => "digraph",
                DotTokenKind.Graph => "graph",
                DotTokenKind.Arrow => "->",
                DotTokenKind.Subgraph => "subgraph",
                DotTokenKind.Node => "node",
                DotTokenKind.Edge => "edge",
                DotTokenKind.Whitespace => " ",
                DotTokenKind.ScopeStart => "{",
                DotTokenKind.ScopeEnd => "}",
                DotTokenKind.Semicolon => ";",
                DotTokenKind.Equal => "=",
                DotTokenKind.ListStart => "[",
                DotTokenKind.ListEnd => "]",
                DotTokenKind.Comma => ",",
                DotTokenKind.Colon => ":",
                DotTokenKind.Quote => "\"",
                DotTokenKind.HtmlStringStart => "<",
                DotTokenKind.HtmlStringEnd => ">",
                _ => null,
            };

            return value is not null;
        }
    }
}
