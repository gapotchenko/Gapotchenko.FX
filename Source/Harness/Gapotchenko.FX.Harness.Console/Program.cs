#region Usings
using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.ComponentModel;
using Gapotchenko.FX.Data.Encoding;
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.IO;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Linq.Expressions;
using Gapotchenko.FX.Math;
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
using System.Threading;
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

        static void _Run()
        {
            Console.WriteLine("Process: {0}", RuntimeInformation.ProcessArchitecture);
            Console.WriteLine("OS: {0}", RuntimeInformation.OSArchitecture);

            Console.WriteLine(BitOperations.Log2(32));

            IEnumerable<int> source = new[] { 1, 2, 3 };

            var h = source.ToHashSet();

            Console.WriteLine(h.IsNullOrEmpty());

            var data = Base32.Instance.EncodeData(new byte[] { 1, 2, 3 });            
        }

        static async Task _RunAsync(CancellationToken ct)
        {
            await Task.Yield();
        }
    }
}
