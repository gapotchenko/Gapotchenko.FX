using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;

namespace Gapotchenko.FX.Data.Storage.Archives.Zip.Tests;

static class Assets
{
    public static Stream OpenStream(string fileName, bool writable = false)
    {
        string path = Path.Combine(
            typeof(Assets).Assembly.GetAssemblyLocation(),
            "../../../../Assets",
            fileName);

        Stream stream = File.OpenRead(path);

        if (writable)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            stream.Dispose();
            stream = memoryStream;
        }

        return stream;
    }
}
