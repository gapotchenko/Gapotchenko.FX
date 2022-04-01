namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc
{
    abstract class TocNode
    {
        public TocNode? Parent { get; set; }

        public List<TocNode> Children { get; } = new List<TocNode>();

        public TocNode? Book =>
            Parent switch
            {
                null => null,
                TocRootNode _ => this,
                _ => Parent.Book
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
}
