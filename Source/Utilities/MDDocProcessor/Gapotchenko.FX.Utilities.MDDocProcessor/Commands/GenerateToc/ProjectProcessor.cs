using Gapotchenko.FX.Text;
using Gapotchenko.FX.Utilities.MDDocProcessor.Framework;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;
using Gapotchenko.FX.Utilities.MDDocProcessor.Serialization;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GenerateToc;

sealed class ProjectProcessor(TocProjectNode tocNode)
{
    public void Run()
    {
        string? filePath = m_TocNode.Project.ReadMeFilePath;
        if (filePath != null)
            UpdateToc(filePath);
    }

    void UpdateToc(string mdFilePath)
    {
        if (!mdFilePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException("Cannot update TOC in a non-markdown file.");

        string text = File.ReadAllText(mdFilePath);

        var tocRegex = new Regex(
            @"[\r\n]+(?<toc>(\s*-\s+(.+\s+\(namespace\)|(&\#x27B4;\s*)?\[(?<name>.+)]\((?<url>.+)\)(\s+✱+)?)\s*?([\r\n]|$)){2,})",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        var tocGroup =
            tocRegex.EnumerateMatchesLinq(text).Select(x => x.Groups["toc"]).Where(IsTocMatch).SingleOrDefault() ??
            throw new InvalidOperationException("Cannot find TOC section.");

        bool IsTocMatch(Group group)
        {
            if (!group.Success)
                return false;

            var toc = m_TocNode.Document;
            if (toc == null)
                return false;

            return true;
        }

        var book = m_TocNode.Book;
        if (book == null)
            throw new InvalidOperationException("TOC book not found.");

        var tocSerializer = new TocSerializer();

        var tocWriter = new StringWriter();
        tocSerializer.SerializeToc(tocWriter, book, m_TocNode);
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

    readonly TocProjectNode m_TocNode = tocNode;
}
