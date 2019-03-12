using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Harness.Console
{
    using Console = System.Console;

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
            var list = new List<int>();

            list.AddRange(10, 20);

            Console.WriteLine(list.IsNullOrEmpty());
        }

        static async Task _RunAsync()
        {
            await Task.Yield();
        }
    }
}
