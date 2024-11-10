namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

interface ITocNode
{
    ITocNode? Parent { get; set; }
    IList<TocNode> Children { get; }
}
