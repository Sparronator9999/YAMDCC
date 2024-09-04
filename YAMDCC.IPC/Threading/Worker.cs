using System;
using System.Threading;
using System.Threading.Tasks;

namespace YAMDCC.IPC.Threading
{
    internal sealed class Worker
    {
        private readonly TaskScheduler _callbackThread;

        private static TaskScheduler CurrentTaskScheduler =>
            SynchronizationContext.Current != null
                ? TaskScheduler.FromCurrentSynchronizationContext()
                : TaskScheduler.Default;

        internal event EventHandler Succeeded;
        internal event EventHandler<WorkerErrorEventArgs> Error;

        internal Worker() : this(CurrentTaskScheduler) { }

        internal Worker(TaskScheduler callbackThread)
        {
            _callbackThread = callbackThread;
        }

        internal void DoWork(Action action)
        {
            new Task(DoWorkImpl, action, CancellationToken.None, TaskCreationOptions.LongRunning).Start();
        }

        internal void DoWorkImpl(object oAction)
        {
            Action action = (Action)oAction;
            try
            {
                action();
                Callback(Succeed);
            }
            catch (Exception e)
            {
                Callback(() => Fail(e));
            }
        }

        private void Succeed()
        {
            Succeeded?.Invoke(this, EventArgs.Empty);
        }

        private void Fail(Exception exception)
        {
            Error?.Invoke(this, new WorkerErrorEventArgs(exception));
        }

        private void Callback(Action action)
        {
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _callbackThread);
        }
    }
}
