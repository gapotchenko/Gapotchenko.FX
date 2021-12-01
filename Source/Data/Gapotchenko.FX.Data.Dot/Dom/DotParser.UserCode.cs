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

        List<DotSyntaxTrivia> _pendingTrivia = new();
        DotSyntaxToken _lastToken;

        protected override int yylex()
        {
            ProcessPendingTrivia();

            _scanner.Read();
            while (IsTriviaToken(_scanner.TokenType))
            {
                switch (_scanner.TokenType)
                {
                    case DotToken.WHITESPACE:
                        ProcessWhitespace(_scanner.Value);
                        break;
                    default:
                        var trivia = CreateTrivia(_scanner.TokenType, _scanner.Value);
                        if (_lastToken is not null)
                            _lastToken.TrailingTrivia.Add(trivia);
                        else
                            _pendingTrivia.Add(trivia);
                        break;
                }
                _scanner.Read();
            }

            _lastToken = CreateToken(_scanner.TokenType, _scanner.Value);
            _yylval = new DotValueType
            {
                token = _lastToken
            };

            return MapToken(_scanner.TokenType);
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

        void ProcessPendingTrivia()
        {
            if (_lastToken != null)
            {
                if (_pendingTrivia.Count > 0)
                {
                    _lastToken.LeadingTrivia.InsertRange(0, _pendingTrivia);
                    _pendingTrivia.Clear();
                }
            }
        }

        void ProcessWhitespace(string value)
        {
            if (_lastToken is null)
            {
                var trivia = CreateWhitespaceTrivia(value);
                _pendingTrivia.Add(trivia);
            }
            else
            {
                var (init, last) = SplitWhitespace(value);
                var trivia = CreateWhitespaceTrivia(init);
                _lastToken.TrailingTrivia.Add(trivia);
                if (last is not null)
                {
                    trivia = CreateWhitespaceTrivia(last);
                    _pendingTrivia.Add(trivia);
                }
            }
        }

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
