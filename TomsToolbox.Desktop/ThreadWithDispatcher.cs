namespace TomsToolbox.Desktop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// A dedicated thread hosting an active dispatcher.
    /// The thread will be created as foreground thread, so this object must be explicitly disposed before the application can shut down.
    /// </summary>
    /// <remarks>
    /// Use this thread to dispatch or serialize background operations, or to host COM objects that don't have a free threaded marshaller.
    /// </remarks>
    public sealed class ForegroundThreadWithDispatcher : ThreadWithDispatcher, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForegroundThreadWithDispatcher"/> class with MTA apartment and normal thread priority.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        public ForegroundThreadWithDispatcher([NotNull] string name)
            : this(name, ApartmentState.MTA)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForegroundThreadWithDispatcher"/> class with normal thread priority.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="state">The apartment state of the thread.</param>
        public ForegroundThreadWithDispatcher([NotNull] string name, ApartmentState state)
            : this(name, state, ThreadPriority.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForegroundThreadWithDispatcher"/> class.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="state">The apartment state of the thread.</param>
        /// <param name="priority">The priority of the thread.</param>
        public ForegroundThreadWithDispatcher([NotNull] string name, ApartmentState state, ThreadPriority priority)
            : base(name, state, priority, false)
        {
            ShutdownPriority = DispatcherPriority.Normal;
        }

        /// <summary>
        /// Gets or sets the shutdown priority passed to <see cref="System.Windows.Threading.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority)"/> when the object is disposed. The default is <see cref="DispatcherPriority.Normal"/>
        /// </summary>
        public DispatcherPriority ShutdownPriority
        {
            get;
            set;
        }

        #region IDisposable Members

        /// <summary>
        /// Shut down the dispatcher and wait for the thread to terminate.
        /// </summary>
        public void Dispose()
        {
            BeginShutdown(ShutdownPriority);
            Join();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ForegroundThreadWithDispatcher"/> class.
        /// </summary>
        ~ForegroundThreadWithDispatcher()
        {
            this.ReportNotDisposedObject();
        }

        #endregion
    }

    /// <summary>
    /// A dedicated thread hosting an active dispatcher.
    /// The thread will be created as background thread, so it does not need to be shut down explicitly.
    /// </summary>
    /// <remarks>
    /// Use this thread to dispatch or serialize background operations, or to performant host COM objects that don't have a free threaded marshaller.
    /// A background thread will be killed by the system when the application terminates, so do not host objects in a background thread that need cleanup!
    /// </remarks>
    public class BackgroundThreadWithDispatcher : ThreadWithDispatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundThreadWithDispatcher"/> class.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="state">The apartment state of the thread.</param>
        public BackgroundThreadWithDispatcher([NotNull] string name, ApartmentState state)
            : base(name, state, ThreadPriority.Normal, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundThreadWithDispatcher"/> class.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="state">The apartment state of the thread.</param>
        /// <param name="priority">The priority of the thread.</param>
        public BackgroundThreadWithDispatcher([NotNull] string name, ApartmentState state, ThreadPriority priority)
            : base(name, state, priority, true)
        {
        }
    }


    /// <summary>
    /// A dedicated thread hosting an active dispatcher.
    /// <para/>
    /// Creates a thread and starts a dispatcher in this thread. The dispatcher is only accessible after it's fully started.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract class ThreadWithDispatcher
    {
        [NotNull]
        private readonly Thread _thread;
        [NotNull]
        private readonly EventWaitHandle _threadStarted = new EventWaitHandle(false, EventResetMode.ManualReset);
        [CanBeNull]
        private Dispatcher _dispatcher;
        [CanBeNull]
        private TaskScheduler _taskScheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadWithDispatcher" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="state">The state.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="isBackgroundThread">if set to <c>true</c> it the thread should be created as background thread.</param>
        protected ThreadWithDispatcher([NotNull] string name, ApartmentState state, ThreadPriority priority, bool isBackgroundThread)
        {
            var currentThread = Thread.CurrentThread;

            _thread = new Thread(ThreadMethod)
            {
                Name = name,
                CurrentCulture = currentThread.CurrentCulture,
                CurrentUICulture = currentThread.CurrentUICulture,
                Priority = priority,
                IsBackground = isBackgroundThread
            };

            _thread.SetApartmentState(state);
            _thread.Start();
        }

        /// <summary>
        /// Gets the dispatcher of the thread.
        /// </summary>
        [NotNull]
        public Dispatcher Dispatcher
        {
            get
            {
                try
                {
                    _threadStarted.WaitOne();
                }
                catch (ObjectDisposedException)
                {
                }
                return _dispatcher;
            }
        }

        /// <summary>
        /// Gets the task scheduler associated with the <see cref="Dispatcher"/>
        /// </summary>
        [NotNull]
        public TaskScheduler TaskScheduler
        {
            get
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return _taskScheduler ?? (_taskScheduler = Invoke(TaskScheduler.FromCurrentSynchronizationContext));
            }
        }

        /// <summary>
        /// Occurs when the dispatcher is terminated.
        /// </summary>
        public event EventHandler<EventArgs> Terminated;

        /// <summary>
        /// Invokes the specified method in the dispatcher thread.
        /// </summary>
        /// <typeparam name="T">The return type of the method.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>The result of the method.</returns>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        /// <remarks>Exceptions thrown by <paramref name="method"/> are passed back to the caller and are not wrapped into a <see cref="TargetInvocationException"/>.</remarks>
        [CanBeNull]
        public T Invoke<T>([NotNull] Func<T> method)
        {
            return Dispatcher.Invoke<T>(method);
        }

        /// <summary>
        /// Invokes the specified method in the dispatcher thread.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        /// <remarks>Exceptions thrown by <paramref name="method"/> are passed back to the caller and are not wrapped into a <see cref="TargetInvocationException"/>.</remarks>
        public void Invoke([NotNull] Action method)
        {
            DispatcherExtensions.Invoke(Dispatcher, method);
        }

        /// <summary>
        /// Invokes the specified method asynchronously in the dispatcher thread.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The dispatcher operation to track the outcome of the call.</returns>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        [NotNull]
        public DispatcherOperation BeginInvoke([NotNull] Action method)
        {
            return BeginInvoke(DispatcherPriority.Normal, method);
        }

        /// <summary>
        /// Invokes the specified method asynchronously in the dispatcher thread.
        /// </summary>
        /// <param name="priority">The priority to use.</param>
        /// <param name="method">The method.</param>
        /// <returns>The dispatcher operation to track the outcome of the call.</returns>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        [NotNull]
        public DispatcherOperation BeginInvoke(DispatcherPriority priority, [NotNull] Action method)
        {
            return Dispatcher.BeginInvoke(method, priority, null);
        }

        /// <summary>
        /// Determines whether the calling thread has access to this <see cref="T:System.Windows.Threading.Dispatcher"/>.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The calling thread does not have access to this <see cref="T:System.Windows.Threading.Dispatcher"/>.
        /// </exception>
        public void VerifyAccess()
        {
            Dispatcher.VerifyAccess();
        }

        /// <summary>
        /// Determines whether the calling thread is the thread associated with this <see cref="T:System.Windows.Threading.Dispatcher"/>.
        /// </summary>
        /// <returns>true if the calling thread is the thread associated with this <see cref="T:System.Windows.Threading.Dispatcher"/>; otherwise, false.</returns>
        public bool CheckAccess()
        {
            return Dispatcher.CheckAccess();
        }

        private void ThreadMethod()
        {
            // Start the dispatcher of the thread and set the started event
            _dispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher.BeginInvoke(new ThreadStart(() => _threadStarted.Set()), DispatcherPriority.Send);

            Dispatcher.Run();

            Terminated?.Invoke(this, EventArgs.Empty);

            _threadStarted.Close();
        }

        /// <summary>
        /// Initiates shutdown of the <see cref="T:System.Windows.Threading.Dispatcher" /> asynchronously.
        /// </summary>
        /// <param name="priority">The priority at which to begin shutdown.</param>
        public void BeginShutdown(DispatcherPriority priority)
        {
            Dispatcher.BeginInvokeShutdown(priority);
        }

        /// <summary>
        /// Blocks the calling thread until the <see cref="T:System.Windows.Threading.Dispatcher"/> terminates.
        /// </summary>
        public void Join()
        {
            _thread.Join();
        }

        /// <summary>
        /// Blocks the calling thread until the <see cref="T:System.Windows.Threading.Dispatcher" /> terminates.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// true if the thread terminated; false if the thread has not terminated after the amount of time specified by the <paramref name="timeout"/> parameter has elapsed.
        /// </returns>
        public bool Join(TimeSpan timeout)
        {
            return _thread.Join(timeout);
        }

        /// <summary>
        /// Aborts this instance.
        /// </summary>
        public void Abort()
        {
            _thread.Abort();
        }
    }
}