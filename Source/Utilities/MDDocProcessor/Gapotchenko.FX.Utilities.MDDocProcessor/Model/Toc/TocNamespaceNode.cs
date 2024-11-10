namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

sealed class TocNamespaceNode(Namespace @namespace) : TocNode, ITocHierarchyItemNode
{
    public string Name => Namespace.Name;

    public Namespace Namespace { get; } = @namespace;

    HierarchyItem ITocHierarchyItemNode.HierarchyItem => Namespace;

    public override string ToString() => Name;
}
