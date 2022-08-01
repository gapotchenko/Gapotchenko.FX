namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

abstract class TocNode : ITocNode
{
    public TocNode? Parent { get; set; }

    ITocNode? ITocNode.Parent
    {
        get => Parent;
        set => Parent = (TocNode?)value;
    }

    public IList<TocNode> Children { get; } = new List<TocNode>();

    public TocNode? Book =>
        Parent switch
        {
            null => null,
            TocRootNode or TocCatalogNode => this,
            _ => Parent.Book
        };

    public TocCatalogNode? Catalog =>
        this switch
        {
            TocCatalogNode catalogNode => catalogNode,
            _ => Parent?.Catalog
        };

    public IEnumerable<TocNode> Ancestors()
    {
        for (var i = Parent; i != null; i = i.Parent)
            yield return i;
    }

    public IEnumerable<TocNode> Descendants()
    {
        foreach (var i in Children)
        {
            yield return i;
            foreach (var j in i.Descendants())
                yield return j;
        }
    }

    public IEnumerable<TocNode> SelfAndDescendants() => Descendants().Prepend(this);

    public virtual TocDocument? Document => Parent?.Document;

    public int Depth => Ancestors().Count();
}
