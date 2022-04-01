#!/usr/bin/env dotnet-script

using System.Globalization;
using System.Text;

if (Args.Count != 2)
{
    Console.WriteLine("Usage: crc-poly-visualize <width> <poly>");
    return 1;
}

static int ParseInt32(ReadOnlySpan<char> s)
{
    if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        return int.Parse(s[2..], NumberStyles.HexNumber);
    else
        return int.Parse(s);
}

static ulong ParseUInt64(ReadOnlySpan<char> s)
{
    if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        return ulong.Parse(s[2..], NumberStyles.HexNumber);
    else
        return ulong.Parse(s);
}

var width = ParseInt32(Args[0]);
var poly = ParseUInt64(Args[1]);

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
