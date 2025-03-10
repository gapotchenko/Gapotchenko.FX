using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;

namespace Gapotchenko.FX.Data.Compression.Zip.Tests;

static class Assets
{
    public static Stream OpenStream(string fileName)
    {
        string path = Path.Combine(
            typeof(Assets).Assembly.GetAssemblyLocation(),
            "../../../../Assets",
            fileName);

        return File.OpenRead(path);
    }

    public static Stream OpenStreamCopy(string fileName)
    {
        using var stream = OpenStream(fileName);
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
}
