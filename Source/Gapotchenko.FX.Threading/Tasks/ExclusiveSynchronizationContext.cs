// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Threading.Utils;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace Gapotchenko.FX.Threading.Tasks;

sealed class ExclusiveSynchronizationContext : SynchronizationContext, IDisposable
{
    readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> m_Queue = new();

    public override void Post(SendOrPostCallback d, object? state) => m_Queue.Add(new(d, state));

    public override SynchronizationContext CreateCopy() => this;

    public void Execute(Func<Task> task)
    {
        Post(
            async _ =>
            {
                try
                {
                    await task().ConfigureAwait(false);
                }
                catch (Exception e)
                {
#if TFF_THREAD_ABORT
                    if (e is ThreadAbortException)
                    {
                        // Allows the task to continue interacting with a task scheduler.
                        ThreadHelper.TryResetAbort();
                    }
#endif

                    Post(
                        static state =>
                        {
                            var edi = (ExceptionDispatchInfo)state!;
                            edi.Throw();
                        },
                        ExceptionDispatchInfo.Capture(e));
                }
                finally
                {
                    End();
                }
            },
            null);

        Loop();
    }

    public void Loop()
    {
        while (m_Queue.TryTake(out var item, Timeout.Infinite))
        {
            // Execute the queued callback.
            item.Key(item.Value);
        }
    }

    void End()
    {
        Post(
            static x => ((ExclusiveSynchronizationContext)x!).m_Queue.CompleteAdding(),
            this);
    }

    public void Dispose()
    {
        m_Queue.Dispose();
    }
}
