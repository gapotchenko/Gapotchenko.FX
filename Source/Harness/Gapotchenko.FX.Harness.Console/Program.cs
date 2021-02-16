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

        struct MyInterval<T2> : IInterval<T2>
        {
            public T2 LowerBound => throw new NotImplementedException();

            public T2 UpperBound => throw new NotImplementedException();

            public bool InclusiveLowerBound => throw new NotImplementedException();

            public bool InclusiveUpperBound => throw new NotImplementedException();

            public bool HasLowerBound => throw new NotImplementedException();

            public bool HasUpperBound => throw new NotImplementedException();

            public IInterval<T2> Interior => throw new NotImplementedException();

            public IInterval<T2> Enclosure => throw new NotImplementedException();

            public bool IsEmpty => throw new NotImplementedException();

            public bool IsSingleton => throw new NotImplementedException();

            public IInterval<T2> Clamp(IInterval<T2> limits)
            {
                throw new NotImplementedException();
            }

            public bool Contains(T2 item)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IInterval<T2> other)
            {
                throw new NotImplementedException();
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

            //var a = new Interval<int>(10, 20, , , );

            //var i2 = new MyInterval<int>();
            //a.Clamp(i2);
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
