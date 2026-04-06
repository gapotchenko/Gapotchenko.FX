// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Tests.Kits;

namespace Gapotchenko.FX.IO.Tests;

[TestClass]
public sealed class FragmentedMemoryStreamTests : StreamTestKit
{
    protected override Stream CreateStream(Stream? content, bool writable)
    {
        var stream = new FragmentedMemoryStream();
        if (content is not null)
        {
            content.CopyTo(stream);
            stream.Position = 0;
        }
        return stream;
    }
}
