namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc
{
    class TocProjectNode : TocNode
    {
        public TocProjectNode(Project project)
        {
            Project = project;
        }

        public Project Project { get; }

        public override string ToString() => Project.ToString();
    }
}
