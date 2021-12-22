#region Usings
using Gapotchenko.FX.AppModel;
using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.ComponentModel;
using Gapotchenko.FX.Console;
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
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

                TaskBridge.Execute(_RunAsync);
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
            var res = new[] { 1, 1, 2 }.CrossJoin(new[] { "A", "B" }, ValueTuple.Create).Distinct();

            Console.WriteLine("A = {0}", Enumerable.Distinct(Permutations.Of(new[] { 1, 1, 2 })).Count());

            Console.WriteLine("Process: {0}", RuntimeInformation.ProcessArchitecture);
            Console.WriteLine("OS: {0}", RuntimeInformation.OSArchitecture);

            Console.WriteLine(BitOperations.Log2(32));

            var process = Process.GetProcessesByName("notepad2").FirstOrDefault();
            if (process != null)
            {
                var env = process.ReadEnvironmentVariables();
                Console.WriteLine(env["PATH"]);
            }

            var seq1 = new[] { "1", "2" };
            var seq2 = new[] { "A", "B", "C" };

            foreach (var i in CartesianProduct.Of(seq1, seq2))
                Console.WriteLine(string.Join(" ", i));

            var g = new Graph<int>();
            g.Edges.Add(1, 2);
            g.Vertices.Add(3);

            foreach (var i in g.Vertices)
            {
            }

            Console.WriteLine(g.ToString("D"));

            AssemblyAutoLoader.Default.AddProbingPath(@"C:\");
            AssemblyAutoLoader.Default.RemoveProbingPath(@"C:\");

            _RunTopologicalSort();
        }

        static void _RunTopologicalSort()
        {
            string seq = "1320";

            static bool Dependency(char a, char b) =>
                (a, b) switch
                {
                    ('1', '0') or
                    ('2', '0') => true,
                    _ => false
                };

            var result = seq.TopologicalOrderBy(Fn.Identity, Dependency);

            Console.WriteLine(string.Concat(result));
        }

        static async Task _RunAsync(CancellationToken ct)
        {
            await Task.Yield();

#if false
            string s = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec suscipit, lectus et dapibus ultricies, sem nulla finibus dolor, vitae pharetra urna risus eget nunc. Nunc laoreet condimentum magna, a varius massa auctor in. Mauris cursus sodales justo eget faucibus. Nullam nec nisi eget lorem faucibus feugiat. Fusce sed iaculis turpis, ut vestibulum ipsum.";

            string filePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Temp\base.txt");

            var stream = Base64.Instance.CreateEncoder(
                File.CreateText(filePath),
                DataEncodingOptions.Indent);
            try
            {
                await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
                await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
                await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
                await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
                await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
                await stream.WriteAsync(Encoding.UTF8.GetBytes(s));
                await stream.WriteAsync(Encoding.UTF8.GetBytes("h"));
            }
            finally
            {
                await stream.DisposeAsync();
            }

            using (var tr = new StreamReader(
                Base64.Instance.CreateDecoder(
                    File.OpenText(filePath),
                    DataEncodingOptions.None)))
            {
                Console.WriteLine(await tr.ReadLineAsync());
            }

            s = Base16.GetString(Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec suscipit"), DataEncodingOptions.Indent | DataEncodingOptions.Wrap);
            Console.WriteLine(s);

            Console.WriteLine(Encoding.UTF8.GetString(Base16.GetBytes(s)));

            Console.WriteLine(Base58.Efficiency);

            //string e = Base64.GetString(Encoding.UTF8.GetBytes(s), DataEncodingOptions.Indent);

            //e = Convert.ToBase64String(Encoding.UTF8.GetBytes(s), Base64FormattingOptions.InsertLineBreaks);

            //Console.WriteLine(e);
#endif

            var appInfo = AppInformation.Current;
            Console.WriteLine(appInfo.ExecutablePath);
            Console.WriteLine(appInfo.Copyright);
            Console.WriteLine(appInfo.ProductName);
            Console.WriteLine(appInfo.InformationalVersion);
        }
    }
}
