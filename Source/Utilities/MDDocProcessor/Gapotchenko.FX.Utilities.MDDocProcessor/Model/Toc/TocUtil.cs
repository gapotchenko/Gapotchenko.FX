using Gapotchenko.FX.Math;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

static class TocUtil
{
    public static ProjectComplexity GetProjectComplexity(TocNode node)
    {
        if (node is TocProjectNode projectNode)
            return projectNode.Project.Complexity;
        else
            return node.Children.Select(GetProjectComplexity).Aggregate((a, b) => System.Math.Min(a, b));
    }
}
