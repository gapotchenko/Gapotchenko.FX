using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Utilities.MDDocProcessor.Framework;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Gapotchenko.FX.Utilities.MDDocProcessor
{
    static class ProjectSerializer
    {
        public static bool IsProjectFolder(string folderPath) => Directory.EnumerateFiles(folderPath, "*.?*proj").Any();

        public static Project ReadProject(string folderPath)
        {
            string? readMeFilePath = Path.Combine(folderPath, "README.md");
            if (!File.Exists(readMeFilePath))
                readMeFilePath = null;

            var complexity = ProjectComplexity.Normal;

            if (readMeFilePath != null)
            {
                string content = File.ReadAllText(readMeFilePath);
                foreach (var text in _EnumerateXmlComments(content))
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

            return new Project
            {
                Name = Path.GetFileName(folderPath),
                FolderPath = folderPath,
                ReadMeFilePath = readMeFilePath,
                Complexity = complexity
            };
        }

        static EvaluateOnce<Regex> _XmlCommentRegex = EvaluateOnce.Create(
            () => new Regex(
                @"<!--\s*(?<content>[\s\S\n]*?)\s*-->",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture));

        static IEnumerable<string> _EnumerateXmlComments(string s) =>
            _XmlCommentRegex.Value.EnumerateMatches(s).Select(m => m.Groups["content"].Value);
    }
}
