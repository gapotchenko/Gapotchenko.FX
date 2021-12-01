#region Usings
using Gapotchenko.FX.AppModel;
using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.ComponentModel;
using Gapotchenko.FX.Console;
using Gapotchenko.FX.Data.Dot.Serialization;
//using Gapotchenko.FX.Data.Encoding;
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.IO;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Linq.Expressions;
using Gapotchenko.FX.Math;
using Gapotchenko.FX.Math.Combinatorics;
using Gapotchenko.FX.Math.Geometry;
using Gapotchenko.FX.Math.Topology;
using Gapotchenko.FX.Reflection;
using Gapotchenko.FX.Text;
using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
#endregion

#nullable enable

namespace Gapotchenko.FX.Harness.Console
{
    using Console = System.Console;
    using Math = System.Math;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (!Console.IsOutputRedirected)
                    Console.OutputEncoding = Encoding.UTF8;

                _Run();

                Console.WriteLine();
                Console.WriteLine("----------------------");
                Console.WriteLine("DONE");
            }
            catch (Exception e)
            {
                Console.Write("Error: ");
                Console.WriteLine(e);
            }
        }

        static void _Run()
        {
            var assetPath = @"C:\Sources\graphviz\rtest\graphs\a.gv";

            using var textReader = new StreamReader(assetPath);
            using var dotReader = DotReader.Create(textReader);

            using var textWriter = new StringWriter();
            using var dotWriter = DotWriter.Create(textWriter);

            while (dotReader.Read())
            {
                var tok = dotReader.TokenType;
                var val = dotReader.Value ?? string.Empty;

                var tokStr = Enum.IsDefined(tok) ? tok.ToString() : ((char)tok).ToString();

                var valEsc = val
                    .Replace("\"", "\\\"")
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n");

                Console.WriteLine($"[{tokStr}] \"{valEsc}\"");

                dotWriter.Write(tok, val);
            }

            Console.WriteLine();

            var outputText = textWriter.ToString();
            var consoleColor = Console.ForegroundColor;
            if (outputText == File.ReadAllText(assetPath))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("NOT OK");
            }

            Console.ForegroundColor = consoleColor;
        }
    }
}
