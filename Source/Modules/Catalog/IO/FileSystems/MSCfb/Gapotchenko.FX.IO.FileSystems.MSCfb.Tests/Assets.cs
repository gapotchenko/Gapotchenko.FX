// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Tests;

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
