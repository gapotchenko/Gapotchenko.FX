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
            var processes = Process.GetProcessesByName("devenv");

            if (processes.Length == 0)
                Console.WriteLine("Process with a given name not found. Please modify the code and specify the existing process name.");

            foreach (var process in processes)
            {
                Console.WriteLine();
                Console.WriteLine("Process with ID {0} has a PATH environment variable:", process.Id);

                var env = process.ReadEnvironmentVariables();

                string path = env["PATH"];
                Console.WriteLine(path);
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
