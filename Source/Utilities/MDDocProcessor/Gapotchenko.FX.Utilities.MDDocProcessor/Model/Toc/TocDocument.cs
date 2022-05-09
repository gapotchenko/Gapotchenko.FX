namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc
{
    sealed class TocDocument
    {
        public TocDocument()
        {
            Root = new TocRootNode(this);
        }

        public TocRootNode Root { get; }
    }
}
