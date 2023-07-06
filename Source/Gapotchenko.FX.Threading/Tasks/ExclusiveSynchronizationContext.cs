using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Security;

namespace Gapotchenko.FX.Threading.Tasks;

sealed class ExclusiveSynchronizationContext : SynchronizationContext
{
    readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> m_Queue = new();

    public override void Post(SendOrPostCallback d, object? state) => m_Queue.Add(new(d, state));

    public override void Send(SendOrPostCallback d, object? state) => d(state);

    public override SynchronizationContext CreateCopy() => this;

    volatile ExceptionDispatchInfo? m_ExceptionDispatchInfo;

    public Func<Exception, bool>? ExceptionFilter { get; set; }

    void Loop()
    {
        while (m_Queue.TryTake(out var task, Timeout.Infinite))
        {
            // Execute the task.
            task.Key(task.Value);

            var edi = m_ExceptionDispatchInfo;
            if (edi != null)
            {
                var fiter = ExceptionFilter;
                if (fiter == null || fiter(edi.SourceException))
                    edi.Throw();
                else
                    m_ExceptionDispatchInfo = null;
            }
        }
    }

    void End() =>
        Post(
            x => ((ExclusiveSynchronizationContext)x!).m_Queue.CompleteAdding(),
            this);

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
                    m_ExceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);

#if TFF_THREAD_ABORT
                    if (e is ThreadAbortException)
                    {
                        try
                        {
                            Thread.ResetAbort();
                        }
                        catch (ThreadStateException)
                        {
                            // Was not aborted with Thread.Abort().
                        }
                        catch (PlatformNotSupportedException)
                        {
                            // Not supported.
                        }
                    }
#endif
                }
                finally
                {
                    End();
                }
            },
            null);

        Loop();
    }
}
