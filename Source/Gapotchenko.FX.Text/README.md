# Gapotchenko.FX.Text

The module provides primitives for string and text processing.

## StringEditor

`StringEditor` class provided by `Gapotchenko.FX.Text` module allows to perform an iterative random-access editing of a string.

It was primarily designed to work in conjunction with `Regex` class from `System.Text.RegularExpressions` namespace to efficiently handle an advanced set of tasks
when functionality provided by conventional methods like `Regex.Replace` is just not enough.

Let's take a look on example:

``` csharp
using Gapotchenko.FX.Text;
using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string input = "This extra complicated overwhelmingly fancy sentence should be deleted. This simple sentence should be preserved. This another demo sentence should be deleted. This rocking sentence should be exclamatory... That's all.";

        var regex = new Regex(
            @"\s*?This .*? sentence should be (?<action>deleted|preserved|exclamatory)(?<end>(\.|!)+)\s*",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        var editor = new StringEditor(input);

        var match = regex.Match(input);
        while (match.Success)
        {
            switch (match.Groups["action"].Value)
            {
                case "deleted":
                    // Remove the match, e.g. the whole sentence.
                    editor.Remove(match);
                    break;

                case "exclamatory":
                    // Change the end of a sentence to an exclamation mark.
                    editor.Replace(match.Groups["end"], "!");
                    break;

                case "preserved":
                    // Do nothing.
                    break;
            }

            // Move to next sentence.
            match = match.NextMatch();
        }

        var output = editor.ToString();
        Console.WriteLine(output);
    }        
}
```

The output of the example:

```
This simple sentence should be preserved. This rocking sentence should be exclamatory! That's all.
```

[More on `StringEditor` class](StringEditor.md#one-shot-editing-operations)

## Other Modules

Let's continue with a look to some other modules provided by Gapotchenko.FX framework:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- &#x27B4; [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.Diagnostics.Process](../Gapotchenko.FX.Diagnostics.Process)
