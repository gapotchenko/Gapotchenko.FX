// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Tests.Kits;

namespace Gapotchenko.FX.IO.Tests;

/// <summary>
/// Tests <see cref="StreamTestKit"/> for correctness by using an existing
/// <see cref="Stream"/> implementation as reference.
/// </summary>
[TestClass]
public sealed class StreamKitTest : StreamTestKit
{
    protected override Stream CreateStream(Stream? content, bool writable)
    {
        if (writable)
        {
            var stream = new MemoryStream();
            if (content is not null)
            {
                content.CopyTo(stream);
                stream.Position = 0;
            }
            return stream;
        }
        else
        {
            byte[] buffer;
            if (content is null)
            {
                buffer = [];
            }
            else
            {
                int size = checked((int)content.Length);
                buffer = new byte[size];
                content.ReadExactly(buffer);
            }

            return new MemoryStream(buffer, false);
        }
    }
}
