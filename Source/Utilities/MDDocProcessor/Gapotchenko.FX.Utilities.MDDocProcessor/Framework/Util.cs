using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Framework;

static class Util
{
    public static string MakeRelativePath(string path, string basePath)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        var baseUri = new Uri(basePath);
        var fullUri = new Uri(path);

        var relativeUri = baseUri.MakeRelativeUri(fullUri);

        string relativePath;
        if (relativeUri.IsAbsoluteUri)
        {
            relativePath = relativeUri.LocalPath;
        }
        else
        {
            // Uri's use forward slashes so convert back to backward slashes
            relativePath = Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
        }

        var relativePath2 = Path.GetRelativePath(Path.GetDirectoryName(basePath), path);

        Debug.Assert(relativePath2 == relativePath);

        return relativePath;
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
