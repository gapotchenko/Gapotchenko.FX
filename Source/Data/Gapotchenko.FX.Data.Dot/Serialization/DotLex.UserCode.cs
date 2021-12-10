using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Serialization
{
    partial class DotLex
    {
        // TBD: 
        // parse string concatenation

        string message = "";

        public int Line => yyline;
        public int Col => yycol;
        public string Message => message;

        public override void yyerror(string format, params object[] args)
        {
            if (args.Length > 0)
                message = string.Format(format, args);
            else
                message = format;
        }

        public void Error(string txt)
        {
            if (txt != null)
            {
                message = string.Format("Unexpected {0}", txt);
            }
        }

        public static DotTokenKind MkId(string txt) => txt.ToLower() switch
        {
            "graph" => DotTokenKind.Graph,
            "digraph" => DotTokenKind.Digraph,
            "subgraph" => DotTokenKind.Subgraph,
            "node" => DotTokenKind.Node,
            "edge" => DotTokenKind.Edge,
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
