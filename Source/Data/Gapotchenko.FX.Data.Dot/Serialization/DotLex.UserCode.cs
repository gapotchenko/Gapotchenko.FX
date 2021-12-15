using System;
using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.Data.Dot.Serialization
{
    partial class DotLex
    {
        public int Line => yyline;
        public int Col => yycol;

        public override void yyerror(string format, params object[] args)
        {
            string message;

            if (args.Length > 0)
                message = string.Format(format, args);
            else
                message = format;

            throw new Exception($"Cannot tokenize document. {message} at line {yyline}, column {yycol}.");
        }

        public void RaiseUnexpectedCharError(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                yyerror("Unexpected char");
            else if (txt.Length is 1)
                yyerror($"Unexpected char \"{txt}\"");
            else
                yyerror($"Unexpected string \"{txt}\"");
        }

        public static DotTokenKind MkId(string txt) => txt.ToLowerInvariant() switch
        {
            "graph" => DotTokenKind.Graph,
            "digraph" => DotTokenKind.Digraph,
            "subgraph" => DotTokenKind.Subgraph,
            "node" => DotTokenKind.Node,
            "edge" => DotTokenKind.Edge,
            "strict" => DotTokenKind.Strict,
            _ => DotTokenKind.Id,
        };

        public enum Tokens
        {
            EOF = -1,
        }

        StringBuilder? _yytextBuilder;

        void BuilderInit()
        {
            if (_yytextBuilder is null)
                _yytextBuilder = new();
            else
                _yytextBuilder.Clear();
        }

        void BuilderAppend()
        {
            Debug.Assert(_yytextBuilder is not null);
            _yytextBuilder.Append(yytext);
        }

        string BuilderBuild()
        {
            Debug.Assert(_yytextBuilder is not null);
            return _yytextBuilder.ToString();
        }
    }
}
