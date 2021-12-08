using System;
using System.Collections.Generic;
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
                message = String.Format(format, args);
            else
                message = format;
        }

        public void Error(string txt)
        {
            if (txt != null)
            {
                message = String.Format("Unexpected {0}", txt);
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

        static string TrimString(string stringId)
        {
            if (stringId.EndsWith("\\\r\n"))
                stringId = stringId.Substring(0, stringId.Length - 3);
            else if (stringId.EndsWith("\\\n"))
                stringId = stringId.Substring(0, stringId.Length - 2);
            return stringId;
        }

        public enum Tokens
        {
            EOF = -1,
        }
    }
}
