namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc
{
    class TocNamespaceNode : TocNode
    {
        public TocNamespaceNode(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}
