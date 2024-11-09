using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Text;
using Gapotchenko.FX.Utilities.MDDocProcessor.Vcs;
using Markdig.Helpers;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GeneratePackageReadMe;

sealed class MarkdownProcessor(MarkdownDocument document, Uri baseUri)
{
    public void Run()
    {
        RemoveMainHeader();
        RemoveShields();
        ReLevelHeaders(-1);

        RemoveSection("Supported Platforms", 1);
        RemoveSection("Usage", 1);
        //RemoveSection("Other Modules", 1);

        PatchUris();

        m_Description = ExtractDescription();
    }

    void ReLevelHeaders(int delta)
    {
        foreach (var i in document.OfType<HeadingBlock>())
            i.Level += delta;
    }

    void RemoveMainHeader()
    {
        var header =
            document.Where(x => x is HeadingBlock hb && hb.Level == 1).ScalarOrDefault() ??
            throw new Exception("Main header not found.");

        document.Remove(header);
    }

    void RemoveShields()
    {
        var links = document.Descendants().OfType<LinkInline>().Where(x => x.Url?.Contains("//img.shields.io/") == true).ToList();
        foreach (var i in links)
        {
            var p = i.Parent;
            p?.Remove();
        }
    }

    bool RemoveSection(string title, int level)
    {
        var header = document
            .OfType<HeadingBlock>()
            .Where(x => x.Level == level && GetText(x) == title)
            .FirstOrDefault();
        if (header == null)
            return false;

        var sectionBody = document
            .SkipWhile(x => x != header)
            .TakeWhile(x => x == header || x is not HeadingBlock)
            .ToList();

        foreach (var i in sectionBody)
            document.Remove(i);

        return true;
    }

    static string? GetText(LeafBlock leafBlock)
    {
        var inline = leafBlock.Inline;
        if (inline == null)
            return null;

        if (inline.First() is not LiteralInline literal)
            return null;

        if (literal.NextSibling != null)
            return null;

        return literal.ToString();
    }

    void PatchUris()
    {
        foreach (var i in document.Descendants().OfType<LinkInline>())
        {
            if (i.Url == null)
                continue;

            var uriUsage = i.IsImage ? RepositoryUriUsage.Resource : RepositoryUriUsage.Link;

            var uri = new Uri(i.Url, UriKind.RelativeOrAbsolute);
            uri = TryMapUri(uri, uriUsage);
            if (uri != null)
            {
                i.Url = uri.ToString();
                i.UnescapedUrl = new StringSlice(i.Url);
            }
        }
    }

    public Uri? TryMapUri(Uri uri, RepositoryUriUsage usage)
    {
        var absoluteUri = new Uri(baseUri, uri);
        var newUri = RepositoryService.TryMapUri(absoluteUri, usage);
        if (newUri != null)
            return newUri;

        return null;
    }

    public string Description =>
        m_Description ??
        throw new InvalidOperationException("Description has not been extracted yet.");

    string? m_Description;

    string ExtractDescription()
    {
        var tw = new StringWriter();

        var elements = document.TakeWhile(x => x is not HeadingBlock).ToArray();

        var mdRenderer = new RoundtripRenderer(tw);
        foreach (var i in elements)
            mdRenderer.Write(i);

        string text = tw.ToString().Trim();
        var se = new StringEditor(text);

        // Remove inline code decorations.
        var regex = new Regex(
            @"(?<start_tag>`).+?(?<end_tag>`)",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        foreach (Match m in regex.Matches(text))
        {
            var g1 = m.Groups["start_tag"];
            var g2 = m.Groups["end_tag"];
            if (g1.Success && g2.Success)
            {
                se.Remove(g1);
                se.Remove(g2);
            }
        }

        // Remove YAML metadata.
        regex = new Regex(
            @"<!--.*?-->",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
        foreach (Match m in regex.Matches(text))
            se.Remove(m);

        regex = new Regex(
            @"^The\s+module\s+provides\s+",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
        foreach (Match m in regex.Matches(text))
            se.Replace(m, "Provides ");

        text = se.ToString().Trim();

        return text;
    }
}
