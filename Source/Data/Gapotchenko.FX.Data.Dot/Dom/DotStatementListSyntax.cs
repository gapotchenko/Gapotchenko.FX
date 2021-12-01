namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotStatementListSyntax : DotSyntaxNode
    {
        public DotSyntaxToken OpenBraceToken { get; set; }

        public DotSyntaxList<DotStatementSyntax> Statements { get; set; }

        public DotSyntaxToken CloseBraceToken { get; set; }
    }
}