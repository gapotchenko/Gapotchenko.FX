using Gapotchenko.FX.Math;
using Gapotchenko.FX.Utilities.MDDocProcessor.Framework;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;
using Gapotchenko.FX.Utilities.MDDocProcessor.Visualization;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Serialization
{
    using Math = System.Math;

    sealed class TocSerializer
    {
        public void SerializeToc(TextWriter textWriter, TocNode rootNode, TocProjectNode? projectNode)
        {
            if (ProjectCompexitySet?.Count > 0)
                ProjectCompexitySet.Clear();
            RootNode = rootNode;
            ProjectNode = projectNode;

            var catalog = rootNode.Catalog?.Catalog;
            var project = projectNode?.Project;

            foreach (var node in rootNode.SelfAndDescendants())
            {
                var nodeProject = (node as TocProjectNode)?.Project;
                bool current = nodeProject == project;

                if (!current && projectNode != null)
                {
                    bool currentParent =
                        node.Ancestors().OfType<TocProjectNode>().Any(x => x.Project == project) ||
                        projectNode.Ancestors().Any(x => x == node.Parent);

                    if (currentParent)
                    {
                        if (node.Depth > projectNode.Depth + 1)
                            continue;
                    }
                    else
                    {
                        if (node.Depth > 2)
                            continue;
                    }

                    if (_GetProjectCompexity(node) > project?.Complexity)
                        continue;
                }

                int indent = Math.Max(node.Depth - rootNode.Depth - 1, 0);
                for (var i = 0; i < indent; ++i)
                    textWriter.Write("  ");

                textWriter.Write("- ");
                if (current && projectNode != null)
                    textWriter.Write("&#x27B4; ");

                var effectiveProject = nodeProject;

                if (effectiveProject == null && node is TocNamespaceNode namespaceNode)
                {
                    effectiveProject = namespaceNode.Children.OfType<TocProjectNode>().FirstOrDefault()?.Project;

                    if (effectiveProject == project)
                        effectiveProject = null;

                    //if (effectiveProject != null)
                    //{
                    //    if (namespaceNode.Children.OfType<TocProjectNode>().Any(x => x.Project == _Project))
                    //        effectiveProject = null;
                    //}
                }

                //if (effectiveProject == null)
                //{
                //    tw.Write(node.ToString());
                //    tw.Write(" (namespace)");
                //}
                //else
                {
                    textWriter.Write('[');
                    textWriter.Write(node.ToString());
                    textWriter.Write(']');

                    textWriter.Write('(');
                    if (effectiveProject != null)
                    {
                        string basePath =
                            project?.ReadMeFilePath ??
                            catalog?.ReadMeFilePath ??
                            throw new Exception("Cannot determine base path.");

                        textWriter.Write(Util.MakeRelativePath(effectiveProject.DirectoryPath, basePath).Replace(Path.DirectorySeparatorChar, '/'));
                    }
                    else
                    {
                        textWriter.Write('#');
                    }
                    textWriter.Write(')');

                    if (effectiveProject != null)
                    {
                        var projectComplexity = effectiveProject.Complexity;

                        int starCount = ProjectComplexityVisualizer.GetStarCount(projectComplexity);
                        if (starCount != 0)
                        {
                            var projectCompexitySet = ProjectCompexitySet ??= new HashSet<ProjectComplexity>();
                            projectCompexitySet.Add(projectComplexity);

                            textWriter.Write(' ');
                            for (int i = 0; i < starCount; ++i)
                                textWriter.Write(ProjectComplexityVisualizer.StarSymbol);
                        }
                    }
                }

                textWriter.WriteLine();
            }
        }

        HashSet<ProjectComplexity>? ProjectCompexitySet;
        TocNode? RootNode;
        TocProjectNode? ProjectNode;

        static ProjectComplexity _GetProjectCompexity(TocNode node)
        {
            if (node is TocProjectNode projectNode)
                return projectNode.Project.Complexity;
            else
                return node.Children.Select(_GetProjectCompexity).Aggregate((a, b) => MathEx.Max(a, b));
        }

        public void SerializeLegend(TextWriter textWriter)
        {
            bool complexityLegend = false;

            if (ProjectCompexitySet?.Count > 0)
            {
                bool first = true;

                foreach (var i in ProjectCompexitySet.OrderBy(x => x))
                {
                    if (first)
                        first = false;
                    else
                        textWriter.WriteLine("  ");

                    switch (i)
                    {
                        case ProjectComplexity.Advanced:
                            textWriter.Write("Symbol ✱ denotes an advanced module.");
                            break;
                        case ProjectComplexity.Expert:
                            textWriter.Write("Symbol ✱✱ denotes an expert module.");
                            break;
                    }
                }

                textWriter.WriteLine();
                textWriter.WriteLine();

                complexityLegend = true;
            }

            //if (!fullToc)
            {
                var catalog = RootNode?.Catalog?.Catalog;
                var project = ProjectNode?.Project;

                if (catalog != null && project?.ReadMeFilePath != null)
                {
                    string path = Path.TrimEndingDirectorySeparator(
                        Util.MakeRelativePath(
                            catalog.DirectoryPath + Path.DirectorySeparatorChar,
                            project.ReadMeFilePath));
                    path = path.Replace(Path.DirectorySeparatorChar, '/');

                    if (complexityLegend)
                        textWriter.WriteLine("Or take a look at the [full list of modules]({0}#available-modules).", path);
                    else
                        textWriter.WriteLine("Or look at the [full list of modules]({0}#available-modules).", path);
                }
            }
        }
    }
}
