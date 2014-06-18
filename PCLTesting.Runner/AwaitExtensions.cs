namespace PCLTesting.Runner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Validation;

    internal static class AwaitExtensions
    {
        internal static TaskSchedulerAwaiter GetAwaiter(this TaskScheduler taskScheduler)
        {
            Requires.NotNull(taskScheduler, "taskScheduler");
            return new TaskSchedulerAwaiter(taskScheduler);
        }

        internal struct TaskSchedulerAwaiter : INotifyCompletion
        {
            private readonly TaskScheduler taskScheduler;

            internal TaskSchedulerAwaiter(TaskScheduler taskScheduler)
            {
                Requires.NotNull(taskScheduler, "taskScheduler");
                this.taskScheduler = taskScheduler;
            }

            public bool IsCompleted
            {
                get { return false; }
            }

            public void OnCompleted(Action action)
            {
                Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.taskScheduler);
            }

            public void GetResult() { }
        }
    }
}
