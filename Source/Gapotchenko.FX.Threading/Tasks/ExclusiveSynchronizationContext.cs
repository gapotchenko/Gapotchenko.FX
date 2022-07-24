using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace Gapotchenko.FX.Threading.Tasks;

sealed class ExclusiveSynchronizationContext : SynchronizationContext
{
    readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> m_Queue = new BlockingCollection<KeyValuePair<SendOrPostCallback, object?>>();

    public override void Post(SendOrPostCallback d, object? state) => m_Queue.Add(new KeyValuePair<SendOrPostCallback, object?>(d, state));

    public override void Send(SendOrPostCallback d, object? state) => d(state);

    public override SynchronizationContext CreateCopy() => this;

    volatile ExceptionDispatchInfo? m_ExceptionDispatchInfo;

    public Func<Exception, bool>? ExceptionFilter { get; set; }

    void Loop()
    {
        while (m_Queue.TryTake(out var task, Timeout.Infinite))
        {
            // Execute task.
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

    void End() => Post(_ => m_Queue.CompleteAdding(), null);

    public void Execute(Func<Task> task)
    {
        Post(
            async (_) =>
            {
                try
                {
                    await task().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    m_ExceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);

#if !NETCOREAPP
                    if (e is ThreadAbortException)
                        Thread.ResetAbort();
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
