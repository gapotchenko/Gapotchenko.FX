#region Usings
using Gapotchenko.FX;
using Gapotchenko.FX.AppModel;
using Gapotchenko.FX.Data.Integrity.Checksum;
using Gapotchenko.FX.Data.Encoding;
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math;
using Gapotchenko.FX.Math.Combinatorics;
using Gapotchenko.FX.Math.Intervals;
using Gapotchenko.FX.Math.Graphs;
using Gapotchenko.FX.Reflection;
using Gapotchenko.FX.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.IO;
using Gapotchenko.FX.Math.Metrics;
#endregion

#nullable enable

namespace Gapotchenko.FX.Harness.Console;

using Console = System.Console;

class Program
{
    static void Main2()
    {
        try
        {
            var mutex = new AsyncLock();
            var cv = new AsyncConditionVariable(mutex);

            int[] arr = [1, 2, 3];

            ThreadPool.QueueUserWorkItem(
                (_) =>
                {
                    using var scope = mutex.EnterScope();
                    using var scope2 = mutex.EnterScope();
                    cv.Notify();
                });

            using var scope1 = mutex.EnterScope();
            using var scope2 = mutex.EnterScope();
            using var scope3 = mutex.EnterScope();
            Console.WriteLine(cv.Wait(1000));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    static void Main(string[] args)
    {
        try
        {
            ExecuteOnce a = new(() => Console.WriteLine("Once!"));
            a.EnsureExecuted();
            a.EnsureExecuted();


            if (!Console.IsOutputRedirected)
                Console.OutputEncoding = Encoding.UTF8;

            //TaskBridge.Execute(_RunAsync);
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

        var interval = Interval.Empty<int>();
        Console.WriteLine(interval.IsEmpty);
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
        //await Task.Yield();

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

        var appInfo = AppInformation.For(typeof(object));
        Console.WriteLine("Product: {0}", appInfo.ProductName);
        Console.WriteLine("Version: {0}", appInfo.ProductVersion);
        Console.WriteLine("Company: {0}", appInfo.CompanyName);
        Console.WriteLine("Copyright: {0}", appInfo.Copyright);
        Console.WriteLine(appInfo.ExecutablePath);

        var e = new AsyncManualResetEvent(true);
        Console.WriteLine(await e.WaitAsync(0));

        using var scope = await AsyncMonitor.For(e).EnterScopeAsync();

        return;

        var mutex = new AsyncLock();

        Console.WriteLine("Running lock/unlock loop...");
        var sw = Stopwatch.StartNew();
        for (ulong i = 0; ; ++i)
        {
            using var d = await mutex.EnterScopeAsync();

            //Console.WriteLine("Iteration: {0}", i);

            if (i % 1000 == 0)
            {
                var t = sw.ElapsedMilliseconds;
                await Console.Out.WriteLineAsync(string.Format("{0} ms per 1000.", t));
                sw.Restart();
            }

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.I:
                        Console.WriteLine("Iteration: {0}", i);
                        break;

                    case ConsoleKey.G:
                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                        break;
                }

            }
        }

        if (await mutex.TryEnterAsync(0))
            mutex.Exit();

        //var t1 = VerifyNesting(1, mutex, 1000000);
        //var t2 = VerifyNesting(2, mutex, 1000000);
        //await Task.WhenAll(t1, t2);
        //return;

        var random = new Random();
        Parallel.ForEach(
            Enumerable.Range(1, 2),
            i =>
            {
                int n;
                //lock (random)
                //    n = random.Next(1, 20);
                n = 10;

                try
                {
                    VerifyNesting(i, mutex, n).Wait();
                    //TaskBridge.Execute(
                    //    async () =>
                    //    {
                    //        await VerifyNesting(i, mutex, n);
                    //    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });

        //var mutex = new AsyncRecursiveMutex();

        //await VerifyNesting(mutex, 1);

        using (await mutex.EnterScopeAsync())
        {
            //using (await mutex.LockScopeAsync())
            //{
            //    await Task.Yield();
            //    await Console.Out.WriteLineAsync("345").ConfigureAwait(false);
            //}

            await Task.Yield();
            await Console.Out.WriteLineAsync("123");
        }
    }

    static async Task VerifyNesting(int id, IAsyncLockable lockable, int recursionDepth)
    {
        //bool wasCanceled = false;
        //try
        //{
        //    await lockable.LockAsync(new CancellationToken(true));
        //}
        //catch (OperationCanceledException)
        //{
        //    wasCanceled = true;
        //}
        //if (!wasCanceled)
        //    throw new InvalidOperationException("F6");

        using (var scope = await lockable.TryEnterScopeAsync(0))
        {
            //await lockable.LockAsync();

            if (!lockable.IsEntered)
                throw new InvalidOperationException("F1");
            Console.WriteLine("Entered #{0}", id);

            for (int i = 0; i < recursionDepth; ++i)
            {
                if (!await lockable.TryEnterAsync(0))
                    throw new InvalidOperationException($"F2.{i}");
                if (!lockable.IsEntered)
                    throw new InvalidOperationException($"F3.{i}");
            }

            await Task.Yield();

            for (int i = 0; i < recursionDepth; ++i)
            {
                lockable.Exit();
                if (!lockable.IsEntered)
                    throw new InvalidOperationException($"F4.{i}");
            }

            Console.WriteLine("Exited");
        }

        //if (lockable.IsLocked)
        //    throw new InvalidOperationException("F5");
    }
}
