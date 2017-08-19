namespace TomsToolbox.Desktop
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading;

    using JetBrains.Annotations;

    /// <summary>
    /// A high resolution timer that runs in a single thread. 
    /// Timer events are triggered with high resolution, but new events will not be triggered unless the previous event is processed.
    /// </summary>
    public sealed class HighResolutionTimer : IDisposable
    {
        [NotNull]
        private readonly Stopwatch _stopwatch = new Stopwatch();
        [NotNull]
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        [NotNull]
        private readonly Action<TimeSpan> _timerCallback;

        private TimeSpan _startTimeStamp;
        [CanBeNull]
        private Thread _timerThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighResolutionTimer"/> class.
        /// </summary>
        /// <param name="timerCallback">The timer callback.</param>
        public HighResolutionTimer([NotNull] Action<TimeSpan> timerCallback)
            : this(timerCallback, TimeSpan.FromSeconds(1))
        {
            Contract.Requires(timerCallback != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighResolutionTimer"/> class.
        /// </summary>
        /// <param name="timerCallback">The timer callback.</param>
        /// <param name="interval">The interval.</param>
        public HighResolutionTimer([NotNull] Action<TimeSpan> timerCallback, TimeSpan interval)
        {
            Contract.Requires(timerCallback != null);

            _timerCallback = timerCallback;
            
            Interval = interval;
            Priority = ThreadPriority.Highest;
            Resolution = TimeSpan.FromMilliseconds(1);
        }

        /// <summary>
        /// Gets or sets the timer interval. The default is 1sec.
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Gets or sets the priority of the timer thread. The default is <see cref="ThreadPriority.Highest"/>
        /// </summary>
        public ThreadPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the maximum timer resolution. The default and lower limit is one millisecond.
        /// </summary>
        public TimeSpan Resolution { get; set; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            lock (_stopwatch)
            {
                if (_timerThread != null)
                    return;

                _stopEvent.Reset();
                _stopwatch.Start();

                _timerThread = new Thread(TimerThreadProc) { Name = "HighResolutionTimer", Priority = Priority };
                _timerThread.Start();
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            lock (_stopwatch)
            {
                _stopEvent.Set();

                if (_timerThread != null)
                {
                    _timerThread.Join();
                    _timerThread = null;
                }

                _stopwatch.Stop();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();

            _stopEvent.Dispose();
        }

        private void TimerThreadProc()
        {
            try
            {
                _startTimeStamp = _stopwatch.Elapsed;

                var nextSampleTimeStamp = _startTimeStamp + Interval;

                while (!_stopEvent.WaitOne(Resolution))
                {
                    var timeStamp = _stopwatch.Elapsed;

                    if (timeStamp < nextSampleTimeStamp)
                        continue;

                    var time = timeStamp - _startTimeStamp;
                    nextSampleTimeStamp += Interval;

                    _timerCallback(time);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        [ContractInvariantMethod, UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_stopwatch != null);
            Contract.Invariant(_stopEvent != null);
            Contract.Invariant(_timerCallback != null);
        }
    }
}
