using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    sealed class DotDomWriter : DotDomWalker, IDisposable
    {
        DotWriter _writer;

        public DotDomWriter(DotWriter dotWriter)
            : base(DotDomWalkerDepth.NodesAndTokens)
        {
            _writer = dotWriter ?? throw new ArgumentNullException(nameof(dotWriter));
        }

        public override void VisitToken(DotSignificantToken token)
        {
            if (token.HasLeadingTrivia)
            {
                foreach (var trivia in token.LeadingTrivia)
                {
                    if (!string.IsNullOrEmpty(trivia.Text))
                    {
                        EmitToken(trivia.Kind, trivia.Text);
                    }
                }
            }

            if (!string.IsNullOrEmpty(token.Text))
            {
                EmitToken(token.Kind, token.Text);
            }

            if (token.HasTrailingTrivia)
            {
                foreach (var trivia in token.TrailingTrivia)
                {
                    if (!string.IsNullOrEmpty(trivia.Text))
                    {
                        EmitToken(trivia.Kind, trivia.Text);
                    }
                }
            }
        }

        char _lastChar = '\0';
        bool _newlineExpected = false;

        void EmitToken(DotTokenKind kind, string text)
        {
            Debug.Assert(!string.IsNullOrEmpty(text));

            if (_newlineExpected && !text.StartsWith("\n") && !text.StartsWith("\r\n"))
            {
                _writer.Write(DotTokenKind.Whitespace, DotFormatter.DefaultEOL);
                _lastChar = '\n';
            }

            _newlineExpected = false;

            switch (kind)
            {
                case DotTokenKind.Comment when !text.EndsWith("\n"):
                    _newlineExpected = true;
                    break;
            }

            if (_lastChar != '\0' &&
                !IsTokenDelimiter(text[0]) &&
                !IsTokenDelimiter(_lastChar))
            {
                _writer.Write(DotTokenKind.Whitespace, " ");
            }

            _writer.Write(kind, text);

            _lastChar = text[text.Length - 1];
        }

        static bool IsTokenDelimiter(char ch) =>
            char.IsWhiteSpace(ch) ||
            char.IsPunctuation(ch) ||
            ch is '=' or '<' or '>';

        public void Dispose()
        {
            if (_writer is not null)
            {
                _writer.Dispose();
                _writer = null!;
            }
        }
    }
}
