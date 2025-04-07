using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests.Kits;

static class Assets
{
    public static Stream OpenStream(string fileName, bool writable = false)
    {
        string path = Path.Combine(
            typeof(Assets).Assembly.GetAssemblyLocation(),
            "../../../../Kits/Assets",
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
