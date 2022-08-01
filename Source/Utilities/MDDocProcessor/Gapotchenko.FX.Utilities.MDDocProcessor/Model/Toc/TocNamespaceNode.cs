namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

sealed class TocNamespaceNode : TocNode, ITocHierarchyItemNode
{
    public TocNamespaceNode(Namespace @namespace)
    {
        Namespace = @namespace;
    }

    public string Name => Namespace.Name;

    public Namespace Namespace { get; }

    HierarchyItem ITocHierarchyItemNode.HierarchyItem => Namespace;

    public override string ToString() => Name;
}
