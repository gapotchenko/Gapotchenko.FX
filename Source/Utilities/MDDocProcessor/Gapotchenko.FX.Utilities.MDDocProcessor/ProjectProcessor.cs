using Gapotchenko.FX.Math;
using Gapotchenko.FX.Text;
using Gapotchenko.FX.Utilities.MDDocProcessor.Framework;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;
using Gapotchenko.FX.Utilities.MDDocProcessor.Visualization;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor
{
    using Math = System.Math;

    class ProjectProcessor
    {
        public ProjectProcessor(
            Project project,
            TocProjectNode tocNode,
            string baseDirectory)
        {
            _Project = project;
            _TocNode = tocNode;
            _BaseDirectory = baseDirectory;
        }

        readonly Project _Project;
        readonly TocProjectNode _TocNode;
        readonly string _BaseDirectory;

        public void Run()
        {
            string? filePath = _Project.ReadMeFilePath;
            if (filePath != null)
                _UpdateToc(filePath);
        }

        bool _IsTocMatch(Group group)
        {
            if (!group.Success)
                return false;

            var toc = _TocNode.Document;
            if (toc == null)
                return false;

            return true;
        }

        static ProjectComplexity _GetProjectCompexity(TocNode node)
        {
            if (node is TocProjectNode projectNode)
                return projectNode.Project.Complexity;
            else
                return node.Children.Select(_GetProjectCompexity).Aggregate((a, b) => MathEx.Max(a, b));
        }

        void _UpdateToc(string mdFilePath)
        {
            if (!mdFilePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException("Cannot update TOC in a non-markdown file.");

            string text = File.ReadAllText(mdFilePath);

            var tocRegex = new Regex(
                @"[\r\n]+(?<toc>(\s*-\s+(.+\s+\(namespace\)|(&\#x27B4;\s*)?\[(?<name>.+)]\((?<url>.+)\)(\s+✱+)?)\s*?([\r\n]|$)){2,})",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            var tocGroup = tocRegex.EnumerateMatches(text).Select(x => x.Groups["toc"]).Where(_IsTocMatch).SingleOrDefault();
            if (tocGroup == null)
                return;

            //Console.WriteLine(".");
            //Console.WriteLine(tocGroup.Value);
            //Console.WriteLine(".");

            //Console.WriteLine();

            bool fullToc = true;
            var projectComplexitySet = new HashSet<ProjectComplexity>();

            var tw = new StringWriter();

            var book = _TocNode.Book;
            if (book == null)
                throw new InvalidOperationException("TOC node book not found.");

            foreach (var node in book.SelfAndDescendants())
            {
                var project = (node as TocProjectNode)?.Project;
                bool current = project == _Project;

                if (!current)
                {
                    bool currentParent =
                        node.Ancestors().OfType<TocProjectNode>().Any(x => x.Project == _Project) ||
                        _TocNode.Ancestors().Any(x => x == node.Parent);

                    if (currentParent)
                    {
                        if (node.Depth > _TocNode.Depth + 1)
                            continue;
                    }
                    else
                    {
                        if (node.Depth > 2)
                            continue;
                    }

                    if (_GetProjectCompexity(node) > _Project.Complexity)
                    {
                        fullToc = false;
                        continue;
                    }
                }

                int indent = Math.Max(node.Depth - 2, 0);
                for (var i = 0; i < indent; ++i)
                    tw.Write("  ");

                tw.Write("- ");
                if (current)
                    tw.Write("&#x27B4; ");

                var effectiveProject = project;

                if (effectiveProject == null && node is TocNamespaceNode namespaceNode)
                {
                    effectiveProject = namespaceNode.Children.OfType<TocProjectNode>().FirstOrDefault()?.Project;

                    if (effectiveProject == _Project)
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
                    tw.Write('[');
                    tw.Write(node.ToString());
                    tw.Write(']');

                    tw.Write('(');
                    if (effectiveProject != null)
                        tw.Write(Util.MakeRelativePath(effectiveProject.FolderPath, _Project.FolderPath + Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/'));
                    else
                        tw.Write('#');
                    tw.Write(')');

                    if (effectiveProject != null)
                    {
                        var projectComplexity = effectiveProject.Complexity;

                        int starCount = ProjectComplexityVisualizer.GetStarCount(projectComplexity);
                        if (starCount != 0)
                        {
                            projectComplexitySet.Add(projectComplexity);

                            tw.Write(' ');
                            for (int i = 0; i < starCount; ++i)
                                tw.Write(ProjectComplexityVisualizer.StarSymbol);
                        }
                    }
                }

                tw.WriteLine();
            }

            //Console.WriteLine(tw);

            string newToc = tw.ToString().TrimEnd('\n', '\r');

            text = StringEditor.Replace(text, tocGroup, newToc);

            // ------------------------------------------------------------------------

            int legendStart = tocGroup.Index + newToc.Length;

            var legendRegex = new Regex(
                @"((?<pp>(\r\n|\n|\r){2,})|[\r\n]+)\s*(?<legend>(.|\r|\n)*?)((\r\n|\n|\r)\#|$)",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            var legendMatch = legendRegex.Match(text, legendStart);
            var legendGroup = legendMatch.Groups["legend"];
            if (!legendGroup.Success)
                throw new Exception("Cannot find TOC legend position.");

            var legendWriter = new StringWriter();

            if (!legendMatch.Groups["pp"].Success)
                legendWriter.WriteLine();

            bool complexityLegend = false;

            if (projectComplexitySet.Any())
            {
                bool first = true;
                foreach (var i in projectComplexitySet.OrderBy(x => x))
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        legendWriter.WriteLine("  ");
                    }

                    switch (i)
                    {
                        case ProjectComplexity.Advanced:
                            legendWriter.Write("Symbol ✱ denotes an advanced module.");
                            break;
                        case ProjectComplexity.Expert:
                            legendWriter.Write("Symbol ✱✱ denotes an expert module.");
                            break;
                    }
                }
                legendWriter.WriteLine();
                legendWriter.WriteLine();

                complexityLegend = true;
            }

            //if (!fullToc)
            {
                string mdFileDirectory = Path.GetDirectoryName(mdFilePath) ?? throw new InvalidOperationException();
                string path = Path.TrimEndingDirectorySeparator(
                    Util.MakeRelativePath(
                        _BaseDirectory + Path.DirectorySeparatorChar,
                        mdFileDirectory + Path.DirectorySeparatorChar));
                path = path.Replace(Path.DirectorySeparatorChar, '/');

                if (complexityLegend)
                    legendWriter.WriteLine("Or take a look at the [full list of modules]({0}#available-modules).", path);
                else
                    legendWriter.WriteLine("Or look at the [full list of modules]({0}#available-modules).", path);
            }

            text = StringEditor.Replace(text, legendGroup, legendWriter.ToString().TrimEnd('\n', '\r'));

            // ------------------------------------------------------------------------

            _WriteAllText(mdFilePath, text);

            //throw new ProgramExitException(1);
        }

        static void _WriteAllText(string filePath, string text)
        {
            using var tr = new StringReader(text);
            using var tw = new StreamWriter(filePath, false, Encoding.UTF8);
            for (; ; )
            {
                string? line = tr.ReadLine();
                if (line == null)
                    break;
                tw.WriteLine(line);
            }
        }
    }
}
