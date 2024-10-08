using Gapotchenko.FX.Utilities.MDDocProcessor.Framework;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Gapotchenko.FX.Utilities.MDDocProcessor;

static partial class ProjectSerializer
{
    public static bool IsProjectDirectory(string directoryPath) => Directory.EnumerateFiles(directoryPath, "*.?*pro").Any();

    public static Project ReadProject(string directoryPath)
    {
        string? readMeFilePath = Path.Combine(directoryPath, "README.md");
        if (!File.Exists(readMeFilePath))
            readMeFilePath = null;

        var complexity = ProjectComplexity.Normal;

        if (readMeFilePath != null)
        {
            string content = File.ReadAllText(readMeFilePath);
            foreach (var text in EnumerateXmlComments(content))
            {
                if (text.StartsWith("<docmeta>", StringComparison.Ordinal))
                {
                    var xdoc = XDocument.Parse(text);

                    switch (xdoc.Root?.Element("complexity")?.Value.Trim().ToLowerInvariant())
                    {
                        case null:
                            break;

                        case "normal":
                            complexity = ProjectComplexity.Normal;
                            break;
                        case "advanced":
                            complexity = ProjectComplexity.Advanced;
                            break;
                        case "expert":
                            complexity = ProjectComplexity.Expert;
                            break;

                        case var s:
                            throw new Exception(
                                string.Format(
                                    "Unexpected '{0}' value of 'complexity' property.",
                                    s));
                    }
                }
            }
        }

        return
            new Project(
                Path.GetFileName(directoryPath),
                directoryPath)
            {
                ReadMeFilePath = readMeFilePath,
                Complexity = complexity
            };
    }

    static IEnumerable<string> EnumerateXmlComments(string s) =>
        XmlCommentRegex().EnumerateMatchesLinq(s).Select(m => m.Groups["content"].Value);

    [GeneratedRegex(@"<!--\s*(?<content>[\s\S\n]*?)\s*-->", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
    private static partial Regex XmlCommentRegex();
}
