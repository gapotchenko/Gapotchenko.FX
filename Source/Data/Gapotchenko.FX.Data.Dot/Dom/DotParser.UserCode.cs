using Gapotchenko.FX.Data.Dot.ParserToolkit;
using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    partial class DotParser
    {
        public DotGraphSyntax Root { get; private set; }

        protected override State[] States => states;
        protected override Rule[] Rules => rules;
        protected override string[] NonTerms => nonTerms;

        DotReader _scanner;

        public DotParser(DotReader scanner)
        {
            _scanner = scanner;
        }

        DotValueType _yylval;

        (DotToken kind, string value) _pendingToken;

        protected override int yylex()
        {
            static (DotToken kind, string value) NextToken(DotReader scanner)
            {
                scanner.Read();
                return (scanner.TokenType, scanner.Value);
            }

            var token = _pendingToken;
            if (token.value is null)
            {
                token = NextToken(_scanner);
            }
            else
            {
                _pendingToken = default;
            }

            List<DotSyntaxTrivia>? leadingTrivia = null;

            while (IsTriviaToken(token.kind))
            {
                var trivia = CreateTrivia(token.kind, token.value);
                (leadingTrivia ??= new()).Add(trivia);
                token = NextToken(_scanner);
            }

            var syntaxToken = CreateToken(token.kind, token.value);
            _yylval = new DotValueType
            {
                token = syntaxToken
            };

            if (leadingTrivia is not null)
            {
                foreach (var trivia in leadingTrivia)
                {
                    syntaxToken.LeadingTrivia.Add(trivia);
                }
            }

            _pendingToken = NextToken(_scanner);
            while (IsTriviaToken(_pendingToken.kind))
            {
                var trivia = CreateTrivia(_pendingToken.kind, _pendingToken.value);
                syntaxToken.TrailingTrivia.Add(trivia);
                _pendingToken = NextToken(_scanner);
            }

            return MapToken(token.kind);
        }

        int MapToken(DotToken token) => token switch
        {
            DotToken.EOF => (int)DotTokens.EOF,
            DotToken.DIGRAPH => (int)DotTokens.DIGRAPH,
            DotToken.GRAPH => (int)DotTokens.GRAPH,
            DotToken.ARROW => (int)DotTokens.ARROW,
            DotToken.SUBGRAPH => (int)DotTokens.SUBGRAPH,
            DotToken.NODE => (int)DotTokens.NODE,
            DotToken.EDGE => (int)DotTokens.EDGE,
            DotToken.ID => (int)DotTokens.ID,

            DotToken.LINECOMMENT => throw new InvalidOperationException(),
            DotToken.MLINECOMMENT => throw new InvalidOperationException(),
            DotToken.WHITESPACE => throw new InvalidOperationException(),

            _ => (int)token
        };

        protected override LexLocation? yylloc => null;

        protected override DotValueType yylval => _yylval;

        protected override void yyerror(string message)
        {
            var (line, col) = _scanner.Location;
            throw new Exception($"Cannot parse document. {message} at {line}:{col}.");
        }

        /**********************/
        /**********************/
        /**********************/

        static bool IsTriviaToken(DotToken token) => token switch
        {
            DotToken.WHITESPACE => true,
            DotToken.LINECOMMENT => true,
            DotToken.MLINECOMMENT => true,
            _ => false
        };

        static (string init, string? last) SplitWhitespace(string value)
        {
            var bound = value.LastIndexOf('\n');
            if (bound is -1 || bound == value.Length - 1)
            {
                return (value, null);
            }
            else
            {
                return (value.Substring(0, bound + 1), value.Substring(bound + 1));
            }
        }
    }
}
