namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

sealed class TocRootNode(TocDocument document) : TocNode
{
    public override TocDocument Document { get; } = document;
}
