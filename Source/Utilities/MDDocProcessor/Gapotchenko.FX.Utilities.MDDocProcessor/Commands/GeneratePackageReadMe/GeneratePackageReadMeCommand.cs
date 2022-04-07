using Gapotchenko.FX.Text;
using Markdig;
using Markdig.Renderers.Roundtrip;
using Mono.Options;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GeneratePackageReadMe
{
    static class GeneratePackageReadMeCommand
    {
        public static void Run(string[] args)
        {
            string[]? commonlyUsedParts = null;

            var options = new OptionSet
            {
                {
                    "commonly-used-parts=",
                    "Specifies a list of commonly used parts separated by a semicolon.",
                    x => commonlyUsedParts = Empty.Nullify(x.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                }
            };

            if (args.Length < 2)
            {
                Console.WriteLine(
                    "Usage: {0} generate-package-readme <input markdown file path> <output directory path> [options]",
                    Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);

                throw new ProgramExitException(1);
            }

            string inputFilePath = args[0];
            string outputDirectoryPath = args[1];

            if (options.Parse(args.Skip(2).ToArray()).Count != 0)
                throw new Exception("Malformed command-line arguments.");

            RunCore(inputFilePath, outputDirectoryPath, commonlyUsedParts);
        }

        static void RunCore(string inputFilePath, string outputDirectoryPath, string[]? commonlyUsedParts)
        {
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
