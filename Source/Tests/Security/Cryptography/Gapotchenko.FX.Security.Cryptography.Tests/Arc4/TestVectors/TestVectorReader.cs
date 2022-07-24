using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Gapotchenko.FX.Security.Cryptography.Tests.Arc4.TestVectors;

internal static class TestVectorReader
{
    private enum State
    {
        KeyLength,
        Key,
        Chunks
    }

    public static TestVector Read(string resourceName)
    {
        resourceName = resourceName.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.');

        var type = typeof(TestVectorReader);
        using var stream = type.Assembly.GetManifestResourceStream(type, resourceName);
        if (stream == null)
            throw new Exception($"Unable to open test vector resource '{resourceName}'.");

        var tv = new TestVector();
        var state = State.KeyLength;

        var tr = new StreamReader(stream);
        for (; ; )
        {
            string? line = tr.ReadLine();
            if (line == null)
                break;

            line = line.Trim();

            if (line.Length == 0)
            {
                // Empty line.
                continue;
            }

            switch (state)
            {
                case State.KeyLength:
                    {
                        const string prefix = "Key length:";
                        if (!line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                            throw new InvalidDataException("Cannot read key length statement.");
                        string s = line.Substring(prefix.Length).Trim(' ', '.');
                        var parts = s.Split(' ');
                        if (parts.Length != 2)
                            throw new InvalidDataException("Cannot parse key length statement.");
                        if (!parts[1].Equals("bits", StringComparison.OrdinalIgnoreCase))
                            throw new InvalidDataException("Unsupported key length measurement unit.");
                        tv.KeyLength = int.Parse(parts[0], NumberFormatInfo.InvariantInfo);
                        state = State.Key;
                    }
                    break;

                case State.Key:
                    {
                        const string prefix = "key:";
                        if (!line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                            throw new InvalidDataException("Cannot read key statement.");
                        string s = line.Substring(prefix.Length).Trim(' ', '.');
                        if (!s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                            throw new InvalidDataException("Cannot parse key statement.");
                        s = s.Substring(2);

                        var key = new List<byte>(s.Length / 2);
                        var sr = new StringReader(s);
                        for (; ; )
                        {
                            var buffer = new char[2];
                            int count = sr.Read(buffer, 0, 2);
                            if (count == 0)
                                break;
                            if (count != 2)
                                throw new InvalidDataException("Cannot parse key value.");
                            key.Add(byte.Parse(new string(buffer), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
                        }

                        if (key.Count * 8 != tv.KeyLength)
                            throw new InvalidDataException("The length of the key does not correspond to the previously specified key length.");

                        tv.Key = key.ToArray();
                        state = State.Chunks;
                    }
                    break;

                case State.Chunks:
                    {
                        var parts = line.Split(new[] { ':' }, 2);
                        if (parts.Length != 2)
                            throw new InvalidDataException("Cannot parse data statement.");

                        var offsetToken = parts[0];
                        var dataToken = parts[1];

                        parts = offsetToken.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 4)
                            throw new InvalidDataException("Cannot parse data offset expression.");
                        if (!parts[0].Equals("DEC", StringComparison.OrdinalIgnoreCase))
                            throw new InvalidDataException("Cannot parse data offset expression: expected 'DEC'.");
                        if (!parts[2].Equals("HEX", StringComparison.OrdinalIgnoreCase))
                            throw new InvalidDataException("Cannot parse data offset expression: expected 'HEX'.");

                        int decOffset = int.Parse(parts[1], NumberStyles.Integer);
                        int hexOffset = int.Parse(parts[3], NumberStyles.HexNumber);
                        if (decOffset != hexOffset)
                            throw new InvalidDataException("Cannot parse data offset expression: DEC offset is not equal to HEX offset.");

                        var chunk = new TestVector.Chunk
                        {
                            Offset = decOffset
                        };

                        parts = dataToken.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        chunk.Data = parts.Select(x => byte.Parse(x, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo)).ToArray();

                        tv.Chunks.Add(chunk);
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        return tv;
    }
}
