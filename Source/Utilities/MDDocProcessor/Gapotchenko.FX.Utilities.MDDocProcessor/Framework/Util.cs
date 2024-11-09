using System.Text;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Framework;

static class Util
{
    [return: NotNullIfNotNull(nameof(path))]
    public static string? MakeRelativePath(string? path, string? baseFilePath)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        var baseDirectoryPath = Path.GetDirectoryName(baseFilePath);
        if (string.IsNullOrEmpty(baseDirectoryPath))
            return path;

        return Path.GetRelativePath(baseDirectoryPath, path);
    }

    public static void WriteAllText(string filePath, string text)
    {
        using var tr = new StringReader(text);
        using var tw = new StreamWriter(filePath, false, Encoding.UTF8);
        for (; ; )
        {
            string? line = tr.ReadLine();
            if (line == null)
                break;
            tw.WriteLine(line);
        }
    }
}
