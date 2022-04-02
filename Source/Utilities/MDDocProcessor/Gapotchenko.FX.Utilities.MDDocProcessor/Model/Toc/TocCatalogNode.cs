namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc
{
    sealed class TocCatalogNode : TocNode
    {
        public TocCatalogNode(Catalog catalog)
        {
            Catalog = catalog;
        }

        public new Catalog Catalog { get; }

        public override string ToString() => Catalog.ToString();
    }
}
