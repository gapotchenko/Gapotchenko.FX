namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotStatementListSyntax : DotSyntaxNode
    {
        public DotSyntaxToken OpenBraceToken { get; }

        public DotSyntaxList<DotStatementSyntax> Statements { get; }

        public DotSyntaxToken CloseBraceToken { get; }
    }
}