using Gapotchenko.FX.Data.Dot.ParserToolkit;
using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    partial class DotParser
    {
        public DotGraphNode? Root { get; private set; }

        public struct DotToken
        {
            public DotToken(DotTokenKind kind, string text)
            {
                Kind = kind;
                Text = text;
                _leadingTrivia = null;
                _trailingTrivia = null;
            }

            public DotTokenKind Kind { get; }
            public string Text { get; }
            public bool IsDefault => Text is null;

            List<DotTrivia>? _leadingTrivia;
            List<DotTrivia>? _trailingTrivia;

            public IEnumerable<DotTrivia> LeadingTrivia => _leadingTrivia ?? Enumerable.Empty<DotTrivia>();

            public IEnumerable<DotTrivia> TrailingTrivia => _trailingTrivia ?? Enumerable.Empty<DotTrivia>();

            public void AddLeadingTrivia(DotTrivia trivia)
            {
                (_leadingTrivia ??= new()).Add(trivia);
            }

            public void AddTrailingTrivia(DotTrivia trivia)
            {
                (_trailingTrivia ??= new()).Add(trivia);
            }
        }

        protected override State[] States => states;
        protected override Rule[] Rules => rules;
        protected override string[] NonTerms => nonTerms;

        readonly DotReader _scanner;

        public DotParser(DotReader scanner)
        {
            _scanner = scanner;
        }

        DotValueType _yylval;

        DotToken _pendingToken;

        const DotTokenKind EOF = (DotTokenKind)DotTokens.EOF;

        protected override int yylex()
        {
            static DotToken NextToken(DotReader scanner)
            {
                if (scanner.Read())
                    return new DotToken(scanner.TokenType, scanner.Value);
                else
                    return new DotToken(EOF, string.Empty);
            }

            var token = _pendingToken;
            if (token.Text is null)
            {
                token = NextToken(_scanner);
            }
            else
            {
                _pendingToken = default;
            }

            List<DotTrivia>? leadingTrivia = null;

            while (IsTriviaToken(token.Kind))
            {
                var trivia = CreateTrivia(token);
                (leadingTrivia ??= new()).Add(trivia);
                token = NextToken(_scanner);
            }

            if (leadingTrivia is not null)
            {
                foreach (var trivia in leadingTrivia)
                {
                    token.AddLeadingTrivia(trivia);
                }
            }

            _pendingToken = NextToken(_scanner);
            while (IsTriviaToken(_pendingToken.Kind))
            {
                var trivia = CreateTrivia(_pendingToken);
                token.AddTrailingTrivia(trivia);
                _pendingToken = NextToken(_scanner);
            }

            _yylval = new DotValueType
            {
                token = token
            };

            return MapToken(token.Kind);
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
