using System.Text;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Framework;

static class Util
{
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
