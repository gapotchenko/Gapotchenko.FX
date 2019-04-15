using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Threading
{
    sealed class ExclusiveSynchronizationContext : SynchronizationContext
    {
        readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> m_Queue = new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

        public override void Post(SendOrPostCallback d, object state) =>
            m_Queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));

        public override void Send(SendOrPostCallback d, object state) => throw new InvalidOperationException();

        public override SynchronizationContext CreateCopy() => this;

        ExceptionDispatchInfo m_ExceptionDispatchInfo;

        public Func<Exception, bool> ExceptionFilter { get; set; }

        void Run()
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

        void Complete() => m_Queue.CompleteAdding();

        public void Execute(Func<Task> task)
        {
            Post(
                async (state) =>
                {
                    try
                    {
                        await task().ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        m_ExceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
                        throw;
                    }
                    finally
                    {
                        Complete();
                    }
                },
                null);

            Run();
        }
    }
}
