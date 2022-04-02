using Gapotchenko.FX.Math;
using Gapotchenko.FX.Text;
using Gapotchenko.FX.Utilities.MDDocProcessor.Framework;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;
using Gapotchenko.FX.Utilities.MDDocProcessor.Serialization;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor
{
    sealed class ProjectProcessor
    {
        public ProjectProcessor(TocProjectNode tocNode, string baseDirectory)
        {
            _TocNode = tocNode;
            _BaseDirectory = baseDirectory;
        }

        readonly TocProjectNode _TocNode;
        readonly string _BaseDirectory;

        public void Run()
        {
            string? filePath = _TocNode.Project.ReadMeFilePath;
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

            var book = _TocNode.Book;
            if (book == null)
                throw new InvalidOperationException("TOC node book not found.");

            var tocWriter = new StringWriter();

            var tocSerializer = new TocSerializer();
            tocSerializer.SerializeToc(tocWriter, book, _TocNode);

            string newToc = tocWriter.ToString().TrimEnd('\n', '\r');

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

            tocSerializer.SerializeLegend(legendWriter);

            text = StringEditor.Replace(text, legendGroup, legendWriter.ToString().TrimEnd('\n', '\r'));

            // ------------------------------------------------------------------------

            Util.WriteAllText(mdFilePath, text);
        }
    }
}
