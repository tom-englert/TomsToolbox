namespace TomsToolbox.Desktop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Threading;

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
        public ForegroundThreadWithDispatcher(string name)
            : this(name, ApartmentState.MTA)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForegroundThreadWithDispatcher"/> class with normal thread priority.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="state">The apartment state of the thread.</param>
        public ForegroundThreadWithDispatcher(string name, ApartmentState state)
            : this(name, state, ThreadPriority.Normal)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForegroundThreadWithDispatcher"/> class.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="state">The apartment state of the thread.</param>
        /// <param name="priority">The priority of the thread.</param>
        public ForegroundThreadWithDispatcher(string name, ApartmentState state, ThreadPriority priority)
            : base(name, state, priority, false)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));

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
        public BackgroundThreadWithDispatcher(string name, ApartmentState state)
            : base(name, state, ThreadPriority.Normal, true)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundThreadWithDispatcher"/> class.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="state">The apartment state of the thread.</param>
        /// <param name="priority">The priority of the thread.</param>
        public BackgroundThreadWithDispatcher(string name, ApartmentState state, ThreadPriority priority)
            : base(name, state, priority, true)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
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
        private readonly Thread _thread;
        private readonly EventWaitHandle _threadStarted = new EventWaitHandle(false, EventResetMode.ManualReset);
        private Dispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadWithDispatcher" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="state">The state.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="isBackgroundThread">if set to <c>true</c> it the thread should be created as background thread.</param>
        protected ThreadWithDispatcher(string name, ApartmentState state, ThreadPriority priority, bool isBackgroundThread)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));

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
        public Dispatcher Dispatcher
        {
            get
            {
                Contract.Ensures(Contract.Result<Dispatcher>() != null);

                try
                {
                    _threadStarted.WaitOne();
                }
                catch (ObjectDisposedException)
                {
                }

                Contract.Assume(_dispatcher != null);
                return _dispatcher;
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
        public T Invoke<T>(Func<T> method)
        {
            Contract.Requires(method != null);

            return Dispatcher.Invoke<T>(method);
        }

        /// <summary>
        /// Invokes the specified method in the dispatcher thread.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        /// <remarks>Exceptions thrown by <paramref name="method"/> are passed back to the caller and are not wrapped into a <see cref="TargetInvocationException"/>.</remarks>
        public void Invoke(Action method)
        {
            Contract.Requires(method != null);

            DispatcherExtensions.Invoke(Dispatcher, method);
        }

        /// <summary>
        /// Invokes the specified method asynchronously in the dispatcher thread.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The dispatcher operation to track the outcome of the call.</returns>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        public DispatcherOperation BeginInvoke(Action method)
        {
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<DispatcherOperation>() != null);

            return BeginInvoke(DispatcherPriority.Normal, method);
        }

        /// <summary>
        /// Invokes the specified method asynchronously in the dispatcher thread.
        /// </summary>
        /// <param name="priority">The priority to use.</param>
        /// <param name="method">The method.</param>
        /// <returns>The dispatcher operation to track the outcome of the call.</returns>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Action method)
        {
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<DispatcherOperation>() != null);

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

            if (Terminated != null)
            {
                Terminated(this, EventArgs.Empty);
            }

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

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_thread != null);
            Contract.Invariant(_threadStarted != null);
        }
    }
}