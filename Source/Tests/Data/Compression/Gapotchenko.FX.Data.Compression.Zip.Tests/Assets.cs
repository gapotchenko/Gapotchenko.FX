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
}
