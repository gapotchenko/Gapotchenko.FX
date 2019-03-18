#region Usings
using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.IO;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math;
using Gapotchenko.FX.Text;
using Gapotchenko.FX.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
#endregion

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
                _Run();
                TaskBridge.Execute(_RunAsync);

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

        static ISynchronizeInvoke _inv;

        static void _Run()
        {
            var list = new List<int>();

            list.AddRange(10, 20);

            Console.WriteLine(list.IsNullOrEmpty());

            //WebBrowser.Start("https://www.gapotchenko.com/");

            //var x = Fn.Aggregate(MathEx.Max, new Version(1, 0), new Version(1, 5), new Version(100, 0), null);
            //Console.WriteLine(x);           

            Console.WriteLine(Fn.Aggregate((x, y) => x + y, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10));


            //Optional<int> b = default;
            //Optional<int> a = 20;
            //Console.WriteLine(a > b);


            //Stream stream;

            //var br = new BitReader(Stream.Null, LittleEndianBitConverter.);

            //EqualityComparer<int>.D
        }

        static void _MyMethod(string s)
        {

        }

        static string _MyMethod()
        {
            return "123";
        }

        static async Task _RunAsync()
        {
            await Task.Yield();
        }
    }
}
