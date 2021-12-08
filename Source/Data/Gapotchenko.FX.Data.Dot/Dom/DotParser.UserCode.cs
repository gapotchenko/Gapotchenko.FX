using Gapotchenko.FX.Data.Dot.ParserToolkit;
using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    partial class DotParser
    {
        public DotGraphNode? Root { get; private set; }

        protected override State[] States => states;
        protected override Rule[] Rules => rules;
        protected override string[] NonTerms => nonTerms;

        readonly DotReader _scanner;

        public DotParser(DotReader scanner)
        {
            _scanner = scanner;
        }

        DotValueType _yylval;

        (DotTokenKind kind, string value) _pendingToken;

        const DotTokenKind EOF = (DotTokenKind)DotTokens.EOF;

        protected override int yylex()
        {
            static (DotTokenKind kind, string value) NextToken(DotReader scanner)
            {
                if (scanner.Read())
                    return (scanner.TokenType, scanner.Value);
                else
                    return (EOF, string.Empty);
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

            List<DotTrivia>? leadingTrivia = null;

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

        static int MapToken(DotTokenKind token) => token switch
        {
            EOF => (int)DotTokens.EOF,
            DotTokenKind.Digraph => (int)DotTokens.DIGRAPH,
            DotTokenKind.Graph => (int)DotTokens.GRAPH,
            DotTokenKind.Arrow => (int)DotTokens.ARROW,
            DotTokenKind.Subgraph => (int)DotTokens.SUBGRAPH,
            DotTokenKind.Node => (int)DotTokens.NODE,
            DotTokenKind.Edge => (int)DotTokens.EDGE,
            DotTokenKind.Id => (int)DotTokens.ID,

            DotTokenKind.ScopeStart => '{',
            DotTokenKind.ScopeEnd => '}',
            DotTokenKind.Semicolon => ';',
            DotTokenKind.Equal => '=',
            DotTokenKind.ListStart => '[',
            DotTokenKind.ListEnd => ']',
            DotTokenKind.Comma => ',',
            DotTokenKind.Colon => ':',
            DotTokenKind.Quote => '"',

            DotTokenKind.Comment => throw new InvalidOperationException(),
            DotTokenKind.MultilineComment => throw new InvalidOperationException(),
            DotTokenKind.Whitespace => throw new InvalidOperationException(),

            _ => throw new ArgumentOutOfRangeException(nameof(token))
        };

        protected override LexLocation? yylloc => null;

        protected override DotValueType yylval => _yylval;

        protected override void yyerror(string message)
        {
            var (line, col) = _scanner.Location;
            throw new Exception($"Cannot parse document. {message} at {line}:{col}.");
        }

        static bool IsTriviaToken(DotTokenKind token) => token switch
        {
            DotTokenKind.Whitespace => true,
            DotTokenKind.Comment => true,
            DotTokenKind.MultilineComment => true,
            _ => false
        };
    }
}
