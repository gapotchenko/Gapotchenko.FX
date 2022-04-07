using Gapotchenko.FX.Text;
using Markdig;
using Markdig.Renderers.Roundtrip;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GeneratePackageReadMe
{
    static class GeneratePackageReadMeCommand
    {
        public static void Run(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(
                    "Usage: {0} generate-package-readme <input markdown file path> <output directory path>",
                    Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));

                throw new ProgramExitException(1);
            }

            string inputFilePath = args[0];
            string outputDirectoryPath = args[1];

            var md = Markdown.Parse(File.ReadAllText(inputFilePath), true);

            var mdProcessor = new MarkdownProcessor(md, new Uri(Path.GetFullPath(inputFilePath)));
            mdProcessor.Run();

            Directory.CreateDirectory(outputDirectoryPath);

            var mdWriter = new StringWriter();
            var mdRenderer = new RoundtripRenderer(mdWriter);
            mdRenderer.Write(md);
            string text = mdWriter.ToString();

            var se = new StringEditor(text);

            PatchHtmlUris(mdProcessor, text, se);
            LowerMarkdownSyntax(text, se);

            text = se.ToString();

            using (var outputFile = File.CreateText(Path.Combine(outputDirectoryPath, "README.md")))
            {
                outputFile.WriteLine("# Overview");
                outputFile.WriteLine();
                outputFile.WriteLine(text.Trim());
            }

            using (var outputFile = File.CreateText(Path.Combine(outputDirectoryPath, "Description.txt")))
            {
                outputFile.Write(mdProcessor.Description);
            }
        }

        static void PatchHtmlUris(MarkdownProcessor mdProcessor, string text, StringEditor se)
        {
            var regex = new Regex(
                "(?:\\<\\s*a.*?\\s+href\\s*=\\s*(?:\"|'))(?<uri>.*?)?(?:(?:\"|').*?\\>)",
                RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

            foreach (Match i in regex.Matches(text))
            {
                var g = i.Groups["uri"];
                if (!g.Success)
                    continue;

                var uri = new Uri(g.Value, UriKind.RelativeOrAbsolute);
                var newUri = mdProcessor.TryMapUri(uri);
                if (newUri != null)
                    se.Replace(g, newUri.ToString());
            }
        }

        static void LowerMarkdownSyntax(string text, StringEditor se)
        {
            var regex = new Regex(
                @"(?<start_tag><ins>).*?(?<end_tag></ins>)",
                RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

            foreach (Match i in regex.Matches(text))
            {
                var startTag = i.Groups["start_tag"];
                var endTag = i.Groups["end_tag"];
                if (startTag.Success && endTag.Success)
                {
                    se.Replace(startTag, "*");
                    se.Replace(endTag, "*");
                }
            }
        }
    }
}
