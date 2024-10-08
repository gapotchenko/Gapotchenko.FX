namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

sealed class TocCatalogNode(Catalog catalog) : TocNode
{
    public new Catalog Catalog { get; } = catalog;

    public override string ToString() => Catalog.ToString();
}
