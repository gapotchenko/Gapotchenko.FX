#!/usr/bin/env dotnet-script

// This utility is used as a sample alphabet generator for text encodings.

var args = Args;

if (args.Count != 2)
{
    Console.WriteLine("Usage: alphabet-gen <alphabet> <size>");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  <alphabet>  The base alphabet to use. Possible values: english, greek.");
    Console.WriteLine("  <size>      The number of symbols in generated alphabet.");
    return 1;
}

try
{
    Run(args);
}
catch (Exception e)
{
    Console.Error.Write("Error: ");
    Console.Error.WriteLine(e.Message);
    return 1;
}

return 0;

static void Run(IList<string> args)
{
    var alphabet = args[0];
    var size = int.Parse(args[1]);

    var englishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

    alphabet =
        alphabet switch
        {
            "english" => englishAlphabet,
            "greek" => "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ0123456789αβγδεζηθικλμνξοπρστυφχψω",
            _ => throw new Exception("Specified unknown base alphabet.")
        };

    alphabet = string.Concat((alphabet + englishAlphabet).Distinct());

    if (size > alphabet.Length)
        throw new Exception("The specified number of symbols in generated alphabet is larger than the size of the base alphabet.");

    Console.WriteLine(string.Concat(alphabet.Take(size)));
}
