using Gapotchenko.FX.Text;
using Gapotchenko.FX.Utilities.MDDocProcessor.Vcs;
using Markdig;
using Markdig.Renderers.Roundtrip;
using Mono.Options;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GeneratePackageReadMe;

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
        m_InputFilePath = Path.GetFullPath(inputFilePath);
        m_OutputDirectoryPath = outputDirectoryPath;
        m_CommonlyUsedParts = commonlyUsedParts;

        m_ModuleName =
            Path.GetFileName(Path.GetDirectoryName(m_InputFilePath)) ??
            throw new Exception("Cannot deduct module name.");
    }

    readonly string m_InputFilePath;
    readonly string m_OutputDirectoryPath;
    readonly string[]? m_CommonlyUsedParts;
    readonly string m_ModuleName;

    void RunCore()
    {
        var md = Markdown.Parse(File.ReadAllText(m_InputFilePath), true);

        var mdProcessor = new MarkdownProcessor(md, new Uri(m_InputFilePath));
        mdProcessor.Run();

        Directory.CreateDirectory(m_OutputDirectoryPath);

        var mdWriter = new StringWriter();
        var mdRenderer = new RoundtripRenderer(mdWriter);
        mdRenderer.Write(md);
        string text = mdWriter.ToString();

        var se = new StringEditor(text);

        PatchHtmlUris(mdProcessor, text, se);

        se.Reset(text = se.ToString());

        LowerMarkdownSyntax(text, se);

        ConvertHtmlToMarkdown(ref text, se);

        text = se.ToString();

        if (m_CommonlyUsedParts != null)
        {
            var section = RenderCommonlyUsedPartsToMarkdown(m_CommonlyUsedParts);

            int insertionPoint = text.IndexOf("# See Also");
            if (insertionPoint == -1)
                insertionPoint = text.IndexOf("# Other Modules");

            if (insertionPoint == -1)
                text = text.TrimEnd() + Environment.NewLine + Environment.NewLine + section;
            else
                text = text.Insert(insertionPoint, section + Environment.NewLine);
        }

        using (var outputFile = File.CreateText(Path.Combine(m_OutputDirectoryPath, "README.md")))
        {
            outputFile.WriteLine("# Overview");
            outputFile.WriteLine();
            outputFile.WriteLine(text.Trim());
        }

        using (var outputFile = File.CreateText(Path.Combine(m_OutputDirectoryPath, "Description.txt")))
        {
            outputFile.Write(mdProcessor.Description);
        }
    }

    string RenderCommonlyUsedPartsToMarkdown(string[] commonlyUsedParts)
    {
        var sb = new StringBuilder("# ")
            .AppendLine(
                m_ModuleName.StartsWith("Gapotchenko.FX.Profiles.") ?
                    "Commonly Used Modules" :
                    "Commonly Used Types")
            .AppendLine();

        foreach (var i in commonlyUsedParts)
            sb.Append("- ").Append('`').Append(i).Append('`').AppendLine();

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
            var newUri = mdProcessor.TryMapUri(uri, RepositoryUriUsage.Link);
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

    static void ConvertHtmlToMarkdown(ref string text, StringEditor se)
    {
        var regex = new Regex(
            @"(?<start_tag><div(\s+.*?)?>)(?<body>.*?)(?<end_tag></div>)",
            RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        foreach (Match i in regex.Matches(text))
        {
            string body = i.Groups["body"].Value;

            var sr = new StringReader(body);
            var sw = new StringWriter();

            for (; ; )
            {
                string? line = sr.ReadLine();
                if (line == null)
                    break;
                sw.WriteLine(line.AsSpan().Trim());
            }

            body = sw.ToString();

            se.Replace(i, body);
        }

        // --------------------------------------------------------------

        se.Reset(text = se.ToString());

        regex = new Regex(
            @"(?<start_tag><a\s+href=""(?<href>.*?)""(\s+title=""(?<title>.*?)"")?(\s+.*?)?>)(?<body>.*?)(?<end_tag></a>)",
            RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        foreach (Match i in regex.Matches(text))
        {
            string href = i.Groups["href"].Value;
            string? title = Empty.Nullify(i.Groups["title"].Value);
            string body = i.Groups["body"].Value;

            var sb = new StringBuilder();
            sb.Append('[').Append(body).Append(']');
            sb.Append('(').Append(href);
            if (title != null)
                sb.Append(" \"").Append(title).Append('"');
            sb.Append(')');

            se.Replace(i, sb.ToString());
        }
    }
}
