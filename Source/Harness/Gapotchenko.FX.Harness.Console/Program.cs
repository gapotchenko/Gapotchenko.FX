#region Usings
using Gapotchenko.FX.AppModel;
using Gapotchenko.FX.Data.Integrity.Checksum;
using Gapotchenko.FX.Data.Encoding;
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math;
using Gapotchenko.FX.Math.Combinatorics;
using Gapotchenko.FX.Math.Topology;
using Gapotchenko.FX.Reflection;
using Gapotchenko.FX.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Linq.Operators;
#endregion

#nullable enable

namespace Gapotchenko.FX.Harness.Console;

using Console = System.Console;

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

            if (args.Length == 1)
            {
                int pid = int.Parse(args[0]);
                var process = Process.GetProcessById(pid);
                var mode = process.End(ProcessEndMode.Interrupt);
                Console.WriteLine("Process end mode: {0}", mode);
            }
        }
        catch (Exception e)
        {
            Console.Write("Error: ");
            Console.WriteLine(e);
        }
    }

    static void _Run()
    {
        var parentProcess = Process.GetCurrentProcess().GetParent() ?? throw new Exception("No parent process.");
        Console.WriteLine("Parent process ID: {0}", parentProcess.Id);
        Console.WriteLine("Parent process command line: {0}", parentProcess.ReadArguments());
        Console.WriteLine("------------------------------------------------------------------");
        foreach (var i in parentProcess.ReadArgumentList())
            Console.WriteLine(i);
        Console.WriteLine("------------------------------------------------------------------");
        foreach (DictionaryEntry i in parentProcess.ReadEnvironmentVariables())
        {
            Console.WriteLine("{0}={1}", i.Key, i.Value);
        }
        Console.WriteLine("------------------------------------------------------------------");

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
        g.Edges.Add(2, 3);

        //g.Vertices.Remove(2);

        foreach (var i in g.Vertices)
        {
            Console.WriteLine(i);
        }

        AssemblyAutoLoader.Default.AddProbingPath(@"C:\");
        AssemblyAutoLoader.Default.RemoveProbingPath(@"C:\");

        _RunTopologicalSort();

        var interval = new Interval<int>(10, 20);
    }

    static void _RunTopologicalSort()
    {
        string seq = "13208795";

        static bool Dependency(char a, char b) =>
            (a, b) switch
            {
                ('1', '0') or
                ('2', '0') => true,
                _ => false
            };

        var result = seq.OrderTopologicallyBy(Fn.Identity, Dependency).ThenBy(x => x);

        Console.WriteLine(string.Concat(result));

        var iterator = Crc16.Standard.CreateIterator();
        iterator = Crc16.Attested.Ccitt.CreateIterator();

        var ha = Crc16.Attested.Ccitt.CreateHashAlgorithm();

        var checksum = Crc8.Standard.ComputeChecksum(Encoding.ASCII.GetBytes("123456789"));
        Console.WriteLine("Checksum = 0x{0:x}", checksum);
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

        //Console.WriteLine(Base58.Efficiency);

        //string e = Base64.GetString(Encoding.UTF8.GetBytes(s), DataEncodingOptions.Indent);

        //e = Convert.ToBase64String(Encoding.UTF8.GetBytes(s), Base64FormattingOptions.InsertLineBreaks);

        //Console.WriteLine(e);
#endif

        var appInfo = AppInformation.Current;
        Console.WriteLine(appInfo.ExecutablePath);
        Console.WriteLine(appInfo.Copyright);
        Console.WriteLine(appInfo.ProductName);
        Console.WriteLine(appInfo.InformationalVersion);

        var cs = new AsyncCriticalSection();
        using (await cs.EnterScopeAsync())
        {


            await Console.Out.WriteLineAsync("123");
        }
    }
}
