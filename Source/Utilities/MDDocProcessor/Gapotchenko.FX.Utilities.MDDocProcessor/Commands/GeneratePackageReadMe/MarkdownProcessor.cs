using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Text;
using Gapotchenko.FX.Utilities.MDDocProcessor.Vcs;
using Markdig.Helpers;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GeneratePackageReadMe
{
    sealed class MarkdownProcessor
    {
        public MarkdownProcessor(MarkdownDocument document, Uri baseUri)
        {
            _Document = document;
            _BaseUri = baseUri;
        }

        readonly MarkdownDocument _Document;
        readonly Uri _BaseUri;

        public void Run()
        {
            _RemoveMainHeader();
            _RemoveShields();
            _ReLevelHeaders(-1);

            _RemoveSection("Supported Platforms", 1);
            _RemoveSection("Usage", 1);
            //_RemoveSection("Other Modules", 1);

            _PatchUris();

            _ExtractDescription();
        }

        void _ReLevelHeaders(int delta)
        {
            foreach (var i in _Document.OfType<HeadingBlock>())
                i.Level += delta;
        }

        void _RemoveMainHeader()
        {
            var header = _Document.Where(x => x is HeadingBlock hb && hb.Level == 1).ScalarOrDefault();
            if (header == null)
                throw new Exception("Main header not found.");
            _Document.Remove(header);
        }

        void _RemoveShields()
        {
            var links = _Document.Descendants().OfType<LinkInline>().Where(x => x.Url?.Contains("//img.shields.io/") == true).ToList();
            foreach (var i in links)
            {
                var p = i.Parent;
                if (p != null)
                    p.Remove();
            }
        }

        static string? _GetText(LeafBlock leafBlock)
        {
            var inline = leafBlock.Inline;
            if (inline == null)
                return null;

            var literal = inline.First() as LiteralInline;
            if (literal == null)
                return null;

            if (literal.NextSibling != null)
                return null;

            return literal.ToString();
        }

        bool _RemoveSection(string title, int level)
        {
            var header = _Document
                .OfType<HeadingBlock>()
                .Where(x => x.Level == level && _GetText(x) == title)
                .FirstOrDefault();
            if (header == null)
                return false;

            var sectionBody = _Document
                .SkipWhile(x => x != header)
                .TakeWhile(x => x == header || x is not HeadingBlock)
                .ToList();

            foreach (var i in sectionBody)
                _Document.Remove(i);

            return true;
        }

        void _PatchUris()
        {
            foreach (var i in _Document.Descendants().OfType<LinkInline>())
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
            var absoluteUri = new Uri(_BaseUri, uri);
            var newUri = RepositoryService.TryMapUri(absoluteUri, usage);
            if (newUri != null)
                return newUri;

            return null;
        }

        string? _Description;

        public string Description => _Description ?? throw new InvalidOperationException("Description has not been extracted yet.");

        void _ExtractDescription()
        {
            var tw = new StringWriter();

            var elements = _Document.TakeWhile(x => x is not HeadingBlock).ToArray();

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

            text = se.ToString().Trim();

            _Description = text;
        }
    }
}
