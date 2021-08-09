namespace TomsToolbox.Essentials
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A <see cref="System.Threading.Tasks.TaskScheduler" /> that queues the tasks an runs them in one dedicated thread.
    /// </summary>
    public sealed class ThreadBoundTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly BlockingCollection<Task> _tasksCollection = new();
        private readonly Thread _thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadBoundTaskScheduler"/> class.
        /// </summary>
        public ThreadBoundTaskScheduler()
        {
            _thread = new Thread(Execute);
            _thread.Start();

            TaskFactory = new TaskFactory(this);
        }

        /// <summary>
        /// Gets the thread identifier of the underlying thread.
        /// </summary>
        public int ThreadId => _thread.ManagedThreadId;

        /// <summary>
        /// Gets the task factory that can be used to enqueue a new task.
        /// </summary>
        public TaskFactory TaskFactory { get; }

        private void Execute()
        {
            foreach (var task in _tasksCollection.GetConsumingEnumerable())
            {
                TryExecuteTask(task);
            }
        }

        /// <summary>
        /// For debugger support only, generates an enumerable of <see cref="T:System.Threading.Tasks.Task" /> instances currently queued to the scheduler waiting to be executed.
        /// </summary>
        /// <returns>
        /// An enumerable that allows a debugger to traverse the tasks currently queued to this scheduler.
        /// </returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasksCollection.ToArray();
        }

        /// <summary>
        /// Queues a <see cref="T:System.Threading.Tasks.Task" /> to the scheduler.
        /// </summary>
        /// <param name="task">The <see cref="T:System.Threading.Tasks.Task" /> to be queued.</param>
        protected override void QueueTask(Task task)
        {
            _tasksCollection.Add(task);
        }

        /// <summary>
        /// Determines whether the provided <see cref="T:System.Threading.Tasks.Task" /> can be executed synchronously in this call, and if it can, executes it.
        /// </summary>
        /// <param name="task">The <see cref="T:System.Threading.Tasks.Task" /> to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">A Boolean denoting whether or not task has previously been queued. If this parameter is True, then the task may have been previously queued (scheduled); if False, then the task is known not to have been queued, and this call is being made in order to execute the task inline without queuing it.</param>
        /// <returns>
        /// A Boolean value indicating whether the task was executed inline.
        /// </returns>
        protected override bool TryExecuteTaskInline(Task? task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _tasksCollection.CompleteAdding();
            _thread.Join();
            _tasksCollection.Dispose();
        }
    }
}
