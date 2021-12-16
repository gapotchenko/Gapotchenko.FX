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

        readonly TokenReader _tokenReader;

        public DotParser(DotReader scanner)
        {
            _tokenReader = new TokenReader(scanner);
        }

        DotValueType _yylval;

        const DotTokenKind EOF = (DotTokenKind)DotTokens.EOF;

        protected override int yylex()
        {
            var token = _tokenReader.Next();

            List<DotTrivia>? leadingTrivia = null;

            while (IsTriviaToken(token.Kind))
            {
                var trivia = CreateTrivia(token);
                (leadingTrivia ??= new()).Add(trivia);
                token = _tokenReader.Next();
            }

            if (leadingTrivia is not null)
            {
                foreach (var trivia in leadingTrivia)
                {
                    token.AddLeadingTrivia(trivia);
                }
            }

            bool consumeTrailingTrivia = true;
            while (consumeTrailingTrivia &&
                IsTriviaToken(_tokenReader.Peek().Kind))
            {
                var nextToken = _tokenReader.Next();

                int indexOfNewline;
                if (nextToken.Kind is DotTokenKind.Whitespace &&
                    (indexOfNewline = nextToken.Text.IndexOf('\n')) != -1)
                {
                    var isEndOfFile = _tokenReader.Peek(1).Kind is EOF;

                    if (!isEndOfFile &&
                        indexOfNewline != nextToken.Text.Length - 1)
                    {
                        // Split the trivia by a newline.

                        var thisLineTrivia = new DotTrivia(
                            nextToken.Kind,
                            nextToken.Text.Substring(0, indexOfNewline + 1));
                        token.AddTrailingTrivia(thisLineTrivia);

                        var nextLineTrivia = new DotToken(
                            nextToken.Kind,
                            nextToken.Text.Substring(indexOfNewline + 1));

                        _tokenReader.Push(nextLineTrivia);
                    }
                    else
                    {
                        // EOF token is absent in the tree, attach trivia to the current token.
                        token.AddTrailingTrivia(CreateTrivia(nextToken));
                    }

                    break;
                }
                else
                {
                    token.AddTrailingTrivia(CreateTrivia(nextToken));
                }
            }

            _yylval = new DotValueType
            {
                token = token
            };

            return MapToken(token.Kind);
        }

        sealed class TokenReader
        {
            public DotReader Scanner { get; }
            readonly LinkedList<DotToken> _queue = new();

            public TokenReader(DotReader scanner)
            {
                Scanner = scanner;
            }

            public DotToken Next()
            {
                if (_queue.Count > 0)
                {
                    var first = _queue.First.Value;
                    _queue.RemoveFirst();
                    return first;
                }

                return ReadNext();
            }

            DotToken ReadNext()
            {
                if (Scanner.Read())
                    return new DotToken(Scanner.TokenType, Scanner.Value);
                else
                    return new DotToken(EOF, string.Empty);
            }

            public DotToken Peek(int depth = 0)
            {
                while (_queue.Count - depth < 1)
                {
                    _queue.AddLast(ReadNext());
                }

                if (depth is 0)
                    return _queue.First.Value;
                else
                    return _queue.ElementAt(depth);
            }

            public void Push(DotToken token)
            {
                _queue.AddFirst(token);
            }
        }

        static int MapToken(DotTokenKind token) => token switch
        {
            EOF => (int)DotTokens.EOF,
            DotTokenKind.Digraph => (int)DotTokens.DIGRAPH,
            DotTokenKind.Graph => (int)DotTokens.GRAPH,
            DotTokenKind.Strict => (int)DotTokens.STRICT,
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
            var (line, col) = _tokenReader.Scanner.Location;
            throw new Exception($"Cannot parse document. {message} at line {line} column {col}.");
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
