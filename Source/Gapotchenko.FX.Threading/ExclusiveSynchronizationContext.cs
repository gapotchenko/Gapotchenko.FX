using System;
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
        bool _Done;

        ExceptionDispatchInfo _ExceptionDispatchInfo;

        public Func<Exception, bool> InnerExceptionFilter
        {
            get;
            set;
        }

        readonly Queue<KeyValuePair<SendOrPostCallback, object>> _Items = new Queue<KeyValuePair<SendOrPostCallback, object>>();

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new InvalidOperationException();
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            var item = new KeyValuePair<SendOrPostCallback, object>(d, state);
            lock (_Items)
            {
                _Items.Enqueue(item);
                Monitor.Pulse(_Items);
            }
        }

        public void EndMessageLoop()
        {
            Post(_ => _Done = true, null);
        }

        public void BeginMessageLoop()
        {
            while (!_Done)
            {
                KeyValuePair<SendOrPostCallback, object> task;

                // Retrieve task.
                lock (_Items)
                {
                    if (_Items.Count > 0)
                    {
                        task = _Items.Dequeue();
                    }
                    else
                    {
                        Monitor.Wait(_Items);
                        continue;
                    }
                }

                // Execute task.
                task.Key(task.Value);

                var edi = _ExceptionDispatchInfo;
                if (edi != null)
                {
                    var fiter = InnerExceptionFilter;
                    if (fiter == null || fiter(edi.SourceException))
                        edi.Throw();
                    else
                        _ExceptionDispatchInfo = null;
                }
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }

        internal void ExecuteAsyncTaskSynchronously(Func<Task> task)
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
                        _ExceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
                        throw;
                    }
                    finally
                    {
                        EndMessageLoop();
                    }
                },
                null);

            BeginMessageLoop();
        }
    }
}
