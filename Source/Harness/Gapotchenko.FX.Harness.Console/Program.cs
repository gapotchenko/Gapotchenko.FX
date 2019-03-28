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
            var tw = new StringWriter();

            foreach (var process in Process.GetProcessesByName("notepad2"))
            {
                var result = process.End();
                Console.WriteLine("PID {0}", process.Id);
                Console.WriteLine("Graceful: {0}", (result & ProcessEndMode.Graceful) != 0);
                Console.WriteLine("Forceful: {0}", (result & ProcessEndMode.Forceful) != 0);
            }
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
