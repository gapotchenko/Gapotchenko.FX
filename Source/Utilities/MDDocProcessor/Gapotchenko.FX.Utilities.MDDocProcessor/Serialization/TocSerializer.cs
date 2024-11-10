using Gapotchenko.FX.Linq.Operators;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;
using Gapotchenko.FX.Utilities.MDDocProcessor.Visualization;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Serialization;

using Math = System.Math;

sealed class TocSerializer
{
    public void SerializeToc(TextWriter textWriter, TocNode rootNode, TocProjectNode? projectNode)
    {
        m_ProjectCompexitySet?.Clear();
        m_RootNode = rootNode;
        m_ProjectNode = projectNode;

        var catalog = rootNode.Catalog?.Catalog;
        var project = projectNode?.Project;

        void SerializeTocCore(TocNode rootNode)
        {
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

                    if (TocUtil.GetProjectCompexity(node) > project?.Complexity)
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
                            Path.GetDirectoryName(project?.ReadMeFilePath) ??
                            Path.GetDirectoryName(catalog?.ReadMeFilePath) ??
                            throw new Exception("Cannot determine base path.");

                        string path =
                            Path.GetRelativePath(basePath, effectiveProject.DirectoryPath)
                            .Replace(Path.DirectorySeparatorChar, '/');

                        textWriter.Write(path);
                        textWriter.Write("#readme");
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
                            (m_ProjectCompexitySet ??= []).Add(projectComplexity);

                            textWriter.Write(' ');
                            for (int i = 0; i < starCount; ++i)
                                textWriter.Write(ProjectComplexityVisualizer.StarSymbol);
                        }
                    }
                }

                textWriter.WriteLine();
            }
        }

        if (rootNode is TocCatalogNode catalogNode)
        {
            foreach (var i in rootNode.Children)
                SerializeTocCore(i);
        }
        else
        {
            SerializeTocCore(rootNode);
        }
    }

    public void SerializeLegend(TextWriter textWriter)
    {
        bool complexityLegend = false;

        if (m_ProjectCompexitySet?.Count > 0)
        {
            bool first = true;

            foreach (var i in m_ProjectCompexitySet.OrderBy(x => x))
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
            var catalog = m_RootNode?.Catalog?.Catalog;
            var project = m_ProjectNode?.Project;

            if (catalog != null &&
                Path.GetDirectoryName(project?.ReadMeFilePath) is not null and var basePath)
            {
                string path =
                    Path.GetRelativePath(basePath, catalog.DirectoryPath)
                    .PipeOperator(Path.TrimEndingDirectorySeparator)
                    .Replace(Path.DirectorySeparatorChar, '/');

                if (complexityLegend)
                    textWriter.WriteLine("Or take a look at the [full list of modules]({0}#readme).", path);
                else
                    textWriter.WriteLine("Or look at the [full list of modules]({0}#readme).", path);
            }
        }
    }

    HashSet<ProjectComplexity>? m_ProjectCompexitySet;
    TocNode? m_RootNode;
    TocProjectNode? m_ProjectNode;
}
