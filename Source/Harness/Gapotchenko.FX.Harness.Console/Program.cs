#region Usings
using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.ComponentModel;
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
            var source = new[]
            {
                new { FirstName = "Alex", LastName = "Cooper" },
                new { FirstName = "John", LastName = "Walker" },
                new { FirstName = "Alex", LastName = "The Great" },
                new { FirstName = "Jeremy", LastName = "Doer" }
            };

            var query = source.DistinctBy(x => x.FirstName);

            foreach (var i in query)
                Console.WriteLine(i);
        }

        static async Task _RunAsync(CancellationToken ct)
        {
            await Console.Out.WriteLineAsync("Hello async world!");

            ct.ThrowIfCancellationRequested();

            await TaskBridge.ExecuteAsync(_SyncMethod, ct);
        }

        static void _SyncMethod()
        {
            Console.WriteLine("Sync");
        }
    }
}
