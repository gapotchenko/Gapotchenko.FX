namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotAttributeListSyntax : DotSyntaxNode
    {
        public DotSyntaxToken OpenBraceToken { get; set; }

        public DotSyntaxList<DotAttributeSyntax> Attributes { get; set; }

        public DotSyntaxToken CloseBraceToken { get; set; }
    }
}