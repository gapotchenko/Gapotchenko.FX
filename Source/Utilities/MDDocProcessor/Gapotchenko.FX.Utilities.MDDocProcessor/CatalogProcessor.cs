using Gapotchenko.FX.Text;
using Gapotchenko.FX.Utilities.MDDocProcessor.Framework;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;
using Gapotchenko.FX.Utilities.MDDocProcessor.Serialization;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor
{
    sealed class CatalogProcessor
    {
        public CatalogProcessor(string directoryPath, TocDocument toc)
        {
            _DirectoryPath = directoryPath;
            _Toc = toc;
        }

        readonly string _DirectoryPath;
        TocDocument _Toc;

        public void Run()
        {
            string readMeFilePath = Path.Combine(_DirectoryPath, "README.md");
            if (File.Exists(readMeFilePath))
                _UpdateToc(readMeFilePath);
        }

        void _UpdateToc(string mdFilePath)
        {
            if (!mdFilePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException("Cannot update TOC in a non-markdown file.");

            string text = File.ReadAllText(mdFilePath);

            var tocRegex = new Regex(
                @"[\r\n]+(?<toc>(\s*-\s+(.+\s+\(namespace\)|(&\#x27B4;\s*)?\[(?<name>.+)]\((?<url>.+)\)(\s+✱+)?)\s*?([\r\n]|$)){2,})",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            var tocGroup = tocRegex.EnumerateMatches(text).Select(x => x.Groups["toc"]).Where(x => x.Success).SingleOrDefault();
            if (tocGroup == null)
                return;

            var book = _Toc.Root.Children.Single();

            //var tocWriter = new StringWriter();

            //var tocSerializer = new TocSerializer();
            //tocSerializer.SerializeToc(tocWriter, book, null);

            //string newToc = tocWriter.ToString().TrimEnd('\n', '\r');

            //text = StringEditor.Replace(text, tocGroup, newToc);

        }
    }
}
