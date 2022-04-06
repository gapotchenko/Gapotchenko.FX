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
                    "Usage: {0} generate-package-readme <input markdown file path> <output markdown file path>",
                    Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));

                throw new ProgramExitException(1);
            }

            string inputFilePath = args[0];
            string outputFilePath = args[1];

            var md = Markdown.Parse(File.ReadAllText(inputFilePath), true);

            var mdProcessor = new MarkdownProcessor(md, new Uri(Path.GetFullPath(inputFilePath)));
            mdProcessor.Run();

            var outputDirectoryPath = Path.GetDirectoryName(outputFilePath);
            if (outputDirectoryPath != null)
                Directory.CreateDirectory(outputDirectoryPath);

            var mdWriter = new StringWriter();
            var mdRenderer = new RoundtripRenderer(mdWriter);
            mdRenderer.Write(md);

            string text = mdWriter.ToString();

            var se = new StringEditor(text);

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

            text = se.ToString();

            using (var outputFile = File.CreateText(outputFilePath))
            {
                outputFile.WriteLine("# Overview");
                outputFile.WriteLine();
                outputFile.WriteLine(text.Trim());
            }
        }
    }
}
