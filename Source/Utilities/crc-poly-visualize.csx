#!/usr/bin/env dotnet-script

// This utility is used for documentation of CRC checksums.

using System.Globalization;
using System.Numerics;

var args = Args;

if (args.Count != 2)
{
    Console.WriteLine("Usage: crc-poly-visualize <width> <poly>");
    return 1;
}

static T ParseNumber<T>(ReadOnlySpan<char> s) where T : INumberBase<T>
{
    var provider = NumberFormatInfo.InvariantInfo;
    if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        return T.Parse(s[2..], NumberStyles.HexNumber, provider);
    else
        return T.Parse(s, provider);
}

var width = ParseNumber<int>(args[0]);
var poly = ParseNumber<ulong>(args[1]);

var sb = new StringBuilder();
sb.Append($"x^{width}");

for (int i = width - 1; i >= 0; --i)
{
    if ((poly & (1ul << i)) != 0)
    {
        sb.Append(" + ");
        if (i == 0)
            sb.Append("1");
        else if (i == 1)
            sb.Append("x");
        else
            sb.Append($"x^{i}");
    }
}

Console.WriteLine(sb.ToString());
