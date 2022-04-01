using Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Visualization
{
    static class TocVisualizer
    {
        public static void Visualize(TocNode node, TextWriter tw)
        {
            _VisualizeCore(node, tw, 0);
        }

        static void _VisualizeCore(TocNode node, TextWriter tw, int indent)
        {
            if (!(node is TocRootNode))
            {
                for (int i = 0; i < indent; ++i)
                    tw.Write("\t");
                tw.Write(node.ToString());

                switch (node)
                {
                    case TocNamespaceNode _:
                        tw.Write(" (namespace)");
                        break;

                    case TocProjectNode projectNode:
                        {
                            int starCount = ProjectComplexityVisualizer.GetStarCount(projectNode.Project.Complexity);
                            if (starCount != 0)
                            {
                                tw.Write(' ');
                                for (int i = 0; i < starCount; ++i)
                                    tw.Write("*");
                            }
                        }
                        break;
                }

                tw.WriteLine();
                ++indent;
            }

            foreach (var child in node.Children)
                _VisualizeCore(child, tw, indent);
        }
    }
}
