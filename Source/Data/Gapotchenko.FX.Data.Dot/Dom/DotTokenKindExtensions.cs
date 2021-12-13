using Gapotchenko.FX.Data.Dot.Serialization;
using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    static class DotTokenKindExtensions
    {
        public static DotTokenKind ToDotTokenKind(this DotInsignificantTokenKind token) =>
           token switch
           {
               DotInsignificantTokenKind.Whitespace => DotTokenKind.Whitespace,
               DotInsignificantTokenKind.Comment => DotTokenKind.Comment,
               DotInsignificantTokenKind.MultilineComment => DotTokenKind.MultilineComment,
               _ => throw new ArgumentOutOfRangeException(nameof(token))
           };

        public static DotInsignificantTokenKind ToDotTriviaKind(this DotTokenKind token) =>
           token switch
           {
               DotTokenKind.Whitespace => DotInsignificantTokenKind.Whitespace,
               DotTokenKind.Comment => DotInsignificantTokenKind.Comment,
               DotTokenKind.MultilineComment => DotInsignificantTokenKind.MultilineComment,
               _ => throw new ArgumentOutOfRangeException(nameof(token))
           };

        public static DotTokenKind ToDotTokenKind(this DotArrowTokenKind token) =>
            token switch
            {
                DotArrowTokenKind.LeftToRight => DotTokenKind.Arrow,
                DotArrowTokenKind.Bidirectional => DotTokenKind.Arrow,
                _ => throw new ArgumentOutOfRangeException(nameof(token))
            };

        public static DotTokenKind ToDotTokenKind(this DotKeywordTokenKind token) =>
            token switch
            {
                DotKeywordTokenKind.Digraph => DotTokenKind.Digraph,
                DotKeywordTokenKind.Graph => DotTokenKind.Graph,
                DotKeywordTokenKind.Subgraph => DotTokenKind.Subgraph,
                DotKeywordTokenKind.Node => DotTokenKind.Node,
                DotKeywordTokenKind.Edge => DotTokenKind.Edge,
                _ => throw new ArgumentOutOfRangeException(nameof(token))
            };

        public static DotKeywordTokenKind ToDotKeywordTokenKind(this DotTokenKind token) =>
            token switch
            {
                DotTokenKind.Digraph => DotKeywordTokenKind.Digraph,
                DotTokenKind.Graph => DotKeywordTokenKind.Graph,
                DotTokenKind.Subgraph => DotKeywordTokenKind.Subgraph,
                DotTokenKind.Node => DotKeywordTokenKind.Node,
                DotTokenKind.Edge => DotKeywordTokenKind.Edge,
                _ => throw new ArgumentOutOfRangeException(nameof(token))
            };

        public static DotTokenKind ToDotTokenKind(this DotPunctuationTokenKind token) =>
            token switch
            {
                DotPunctuationTokenKind.ScopeStart => DotTokenKind.ScopeStart,
                DotPunctuationTokenKind.ScopeEnd => DotTokenKind.ScopeEnd,
                DotPunctuationTokenKind.Semicolon => DotTokenKind.Semicolon,
                DotPunctuationTokenKind.Equal => DotTokenKind.Equal,
                DotPunctuationTokenKind.ListStart => DotTokenKind.ListStart,
                DotPunctuationTokenKind.ListEnd => DotTokenKind.ListEnd,
                DotPunctuationTokenKind.Comma => DotTokenKind.Comma,
                DotPunctuationTokenKind.Colon => DotTokenKind.Colon,
                _ => throw new ArgumentOutOfRangeException(nameof(token))
            };

        public static DotPunctuationTokenKind ToDotPunctuationTokenKind(this DotTokenKind token) =>
            token switch
            {
                DotTokenKind.ScopeStart => DotPunctuationTokenKind.ScopeStart,
                DotTokenKind.ScopeEnd => DotPunctuationTokenKind.ScopeEnd,
                DotTokenKind.Semicolon => DotPunctuationTokenKind.Semicolon,
                DotTokenKind.Equal => DotPunctuationTokenKind.Equal,
                DotTokenKind.ListStart => DotPunctuationTokenKind.ListStart,
                DotTokenKind.ListEnd => DotPunctuationTokenKind.ListEnd,
                DotTokenKind.Comma => DotPunctuationTokenKind.Comma,
                DotTokenKind.Colon => DotPunctuationTokenKind.Colon,
                _ => throw new ArgumentOutOfRangeException(nameof(token))
            };
    }
}
