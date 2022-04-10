namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc
{
    sealed class TocRootNode : TocNode
    {
        public TocRootNode(TocDocument document)
        {
            Document = document;
        }

        public override TocDocument Document { get; }
    }
}
