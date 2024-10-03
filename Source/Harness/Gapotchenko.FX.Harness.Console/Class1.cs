using Gapotchenko.FX.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Harness.Console;

using Console = System.Console;

static class Class1
{
    static async Task<int> RunAsync()
    {
        var lockObj = new AsyncLock();

        using (var scope = await lockObj.TryEnterScopeAsync(10))
        {
            await Console.Out.WriteLineAsync("123");
        }

        return 10;
    }
}
