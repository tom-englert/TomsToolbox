namespace TomsToolbox.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;

    /// <summary>
    /// A simple weak event source implementation; useful for static events where you don't want to keep a reference to the event sink.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
    /// <example>
    /// Use like this:
    /// <code language="C#">
    /// <![CDATA[
    /// class SampleSource
    /// {
    ///     private readonly WeakEventSource<EventArgs> _source = new WeakEventSource<EventArgs>();
    /// 
    ///     public event EventHandler AnyAction
    ///     {
    ///         add => _source.Subscribe(value);
    ///         remove => _source.Unsubscribe(value);
    ///     }
    /// 
    ///     private void OnAnyAction()
    ///     {
    ///         _source.Raise(this, EventArgs.Empty);
    ///     }
    /// }
    /// 
    /// class SampleSink
    /// {
    ///     public SampleSink()
    ///     {
    ///         var source = new SampleSource();
    ///         source.AnyAction += Source_AnyAction;
    ///     }
    /// 
    ///     private void Source_AnyAction(object sender, EventArgs e)
    ///     {
    ///         ... do something
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class WeakEventSource<TEventArgs>
        where TEventArgs : EventArgs
    {
        [NotNull, ItemNotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        private readonly List<WeakDelegate> _handlers = new List<WeakDelegate>();

        /// <summary>
        /// Raises the event with the specified sender and argument.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see paramref="TEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void Raise(object sender, TEventArgs e)
        {
            lock (_handlers)
            {
                var garbageCollectedHandlers = _handlers
                    .Where(h => !h.Invoke(sender, e))
                    .ToArray();

                _handlers.RemoveRange(garbageCollectedHandlers);
            }
        }

        /// <summary>
        /// Subscribes the specified handler for the event.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void Subscribe([NotNull] EventHandler<TEventArgs> handler)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var weakHandlers = handler
                .GetInvocationList()
                // ReSharper disable once AssignNullToNotNullAttribute
                .Select(d => new WeakDelegate(d))
                .ToArray();

            lock (_handlers)
            {
                _handlers.AddRange(weakHandlers);
            }
        }

        /// <summary>
        /// Unsubscribes the specified handler from the event.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void Unsubscribe([NotNull] EventHandler<TEventArgs> handler)
        {
            lock (_handlers)
            {
                while (true)
                {
                    var index = _handlers.FindIndex(h1 => h1.Matches(handler));
                    if (index < 0)
                        return;

                    _handlers.RemoveAt(index);
                }
            }

        }

        private class WeakDelegate
        {
            [CanBeNull]
            private readonly WeakReference _weakTarget;
            [NotNull]
            private readonly MethodInfo _method;

            public WeakDelegate([NotNull] Delegate handler)
            {
                _weakTarget = handler.Target != null ? new WeakReference(handler.Target) : null;
                // ReSharper disable once AssignNullToNotNullAttribute
                _method = handler.GetMethodInfo();
            }

            public bool Invoke(object sender, TEventArgs e)
            {
                object target = null;

                if (_weakTarget != null)
                {
                    target = _weakTarget.Target;
                    if (target == null)
                        return false;
                }

                _method.Invoke(target, new[] { sender, e });

                return true;
            }

            public bool Matches([NotNull] EventHandler<TEventArgs> handler)
            {
                return ReferenceEquals(handler.Target, _weakTarget?.Target)
                       && Equals(handler.GetMethodInfo(), _method);
            }
        }
    }
}
