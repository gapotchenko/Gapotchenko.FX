using Gapotchenko.FX.Text;
using Markdig;
using Markdig.Renderers.Roundtrip;
using Mono.Options;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GeneratePackageReadMe
{
    class GeneratePackageReadMeCommand
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

            var command = new GeneratePackageReadMeCommand(inputFilePath, outputDirectoryPath, commonlyUsedParts);
            command.RunCore();
        }

        GeneratePackageReadMeCommand(
            string inputFilePath,
            string outputDirectoryPath,
            string[]? commonlyUsedParts)
        {
            _InputFilePath = Path.GetFullPath(inputFilePath);
            _OutputDirectoryPath = outputDirectoryPath;
            _CommonlyUsedParts = commonlyUsedParts;

            _ModuleName =
                Path.GetFileName(Path.GetDirectoryName(_InputFilePath)) ??
                throw new Exception("Cannot deduct module name.");
        }

        readonly string _InputFilePath;
        readonly string _OutputDirectoryPath;
        readonly string[]? _CommonlyUsedParts;
        readonly string _ModuleName;

        void RunCore()
        {
            var md = Markdown.Parse(File.ReadAllText(_InputFilePath), true);

            var mdProcessor = new MarkdownProcessor(md, new Uri(_InputFilePath));
            mdProcessor.Run();

            Directory.CreateDirectory(_OutputDirectoryPath);

            var mdWriter = new StringWriter();
            var mdRenderer = new RoundtripRenderer(mdWriter);
            mdRenderer.Write(md);
            string text = mdWriter.ToString();

            var se = new StringEditor(text);

            PatchHtmlUris(mdProcessor, text, se);
            LowerMarkdownSyntax(text, se);

            text = se.ToString();

            if (_CommonlyUsedParts != null)
            {
                var section = RenderCommonlyUsedPartsToMarkdown(_CommonlyUsedParts);

                int insertionPoint = text.IndexOf("# See Also");
                if (insertionPoint == -1)
                    insertionPoint = text.IndexOf("# Other Modules");

                if (insertionPoint == -1)
                    text = text.TrimEnd() + Environment.NewLine + Environment.NewLine + section;
                else
                    text = text.Insert(insertionPoint, section + Environment.NewLine);
            }

            using (var outputFile = File.CreateText(Path.Combine(_OutputDirectoryPath, "README.md")))
            {
                outputFile.WriteLine("# Overview");
                outputFile.WriteLine();
                outputFile.WriteLine(text.Trim());
            }

            using (var outputFile = File.CreateText(Path.Combine(_OutputDirectoryPath, "Description.txt")))
            {
                outputFile.Write(mdProcessor.Description);
            }
        }

        string RenderCommonlyUsedPartsToMarkdown(string[] commonlyUsedParts)
        {
            var sb = new StringBuilder("# ")
                .AppendLine(
                    _ModuleName.StartsWith("Gapotchenko.FX.Profiles.") ?
                        "Commonly Used Modules" :
                        "Commonly Used Types")
                .AppendLine();

            foreach (var i in commonlyUsedParts)
                sb.Append("- ").AppendLine(i);

            return sb.ToString();
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
