namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

class TocProjectNode : TocNode, ITocHierarchyItemNode
{
    public TocProjectNode(Project project)
    {
        Project = project;
    }

    public Project Project { get; }

    HierarchyItem ITocHierarchyItemNode.HierarchyItem => Project;

    public override string ToString() => Project.ToString();
}
