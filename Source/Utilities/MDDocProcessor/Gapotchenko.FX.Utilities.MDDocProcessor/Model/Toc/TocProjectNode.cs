namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

class TocProjectNode(Project project) : TocNode, ITocHierarchyItemNode
{
    public Project Project { get; } = project;

    HierarchyItem ITocHierarchyItemNode.HierarchyItem => Project;

    public override string ToString() => Project.ToString();
}
