using Gapotchenko.FX.Data.Dot.Serialization;
using System;

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

        public override void VisitToken(DotToken token)
        {
            if (token.HasLeadingTrivia)
            {
                foreach (var trivia in token.LeadingTrivia)
                {
                    if (!string.IsNullOrEmpty(trivia.Text))
                    {
                        _writer.Write(trivia.Kind, trivia.Text);
                    }
                }
            }

            if (!string.IsNullOrEmpty(token.Text))
            {
                _writer.Write(token.Kind, token.Text);
            }

            if (token.HasTrailingTrivia)
            {
                foreach (var trivia in token.TrailingTrivia)
                {
                    if (!string.IsNullOrEmpty(trivia.Text))
                    {
                        _writer.Write(trivia.Kind, trivia.Text);
                    }
                }
            }
        }

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
