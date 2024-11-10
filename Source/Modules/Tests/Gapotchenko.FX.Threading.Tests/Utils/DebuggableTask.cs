using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Tests.Utils;

static class DebuggableTask
{
    public static async Task WhenAll(params Task[] tasks)
    {
        if (Debugger.IsAttached)
        {
            foreach (var task in tasks)
                await task.ConfigureAwait(false);
        }
        else
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
