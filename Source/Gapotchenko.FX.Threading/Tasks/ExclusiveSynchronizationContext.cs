// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Threading.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Gapotchenko.FX.Threading.Tasks;

sealed class ExclusiveSynchronizationContext : SynchronizationContext, IDisposable
{
    readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> m_Queue = new();

    public override void Post(SendOrPostCallback d, object? state)
    {
        try
        {
            m_Queue.Add(new(d, state));
        }
        catch (ObjectDisposedException)
        {
            Debug.Fail("Asynchronous task cannot execute after the synchronization context has been disposed.");

            // Do no rethrow the exception because it may crash the whole process.
        }
    }

    public override SynchronizationContext CreateCopy() => this;

    volatile bool m_Started;

    public void Start(Func<Task> task)
    {
        if (m_Started)
            throw new InvalidOperationException();

#if TFF_CER
        // Execute the chunk of work related to an asynchronous task in a constrained region.
        RuntimeHelpers.PrepareConstrainedRegionsNoOP();
        try
        {
        }
        finally
#endif
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

            m_Started = true;
        }
    }

    public void Run()
    {
#if !TFF_THREAD_ABORT
        Debug.Assert(m_Started);
#endif

        if (!m_Started)
            return;

        while (m_Queue.TryTake(out var item, Timeout.Infinite))
        {
#if TFF_CER
            // Execute the chunk of work related to an asynchronous task in a constrained region
            // because the task cannot interact with a task scheduler after the thread is aborted.
            RuntimeHelpers.PrepareConstrainedRegionsNoOP();
            try
            {
            }
            finally
#endif
            {
                // Execute the queued callback.
                item.Key(item.Value);
            }
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
