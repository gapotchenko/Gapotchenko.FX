using Gapotchenko.FX.Math;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

static class TocUtil
{
    public static ProjectComplexity GetProjectCompexity(TocNode node)
    {
        if (node is TocProjectNode projectNode)
            return projectNode.Project.Complexity;
        else
            return node.Children.Select(GetProjectCompexity).Aggregate((a, b) => MathEx.Min(a, b));
    }
}
