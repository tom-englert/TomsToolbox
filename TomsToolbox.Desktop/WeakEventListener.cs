namespace TomsToolbox.Desktop
{
    using System;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// Common interface for weak event listener.
    /// </summary>
    public interface IWeakEventListener
    {
        /// <summary>
        /// Detaches from the subscribed event.
        /// </summary>
        void Detach();
    }

    /// <summary>
    /// Implements a weak event listener that allows the owner to be garbage
    /// collected if its only remaining link is an event handler.
    /// </summary>
    /// <typeparam name="TTarget">Type of the target instance listening for the event.</typeparam>
    /// <typeparam name="TSource">Type of source instance for the event.</typeparam>
    /// <typeparam name="TEventArgs">Type of event arguments for the event.</typeparam>
    public class WeakEventListener<TTarget, TSource, TEventArgs> : IWeakEventListener
        where TTarget : class
        where TSource : class
    {
        /// <summary>
        /// WeakReference to the object listening for the event.
        /// </summary>
        [NotNull]
        private readonly WeakReference<TTarget> _weakTarget;

        /// <summary>
        /// To hold only a reference to source object. With this instance the WeakEventListener
        /// can guarantee that the handler gets unregistered when listener is released but does not reference the source.
        /// </summary>
        [CanBeNull]
        private readonly WeakReference<TSource> _weakSource;
        /// <summary>
        /// To hold a reference to source object. With this instance the WeakEventListener
        /// can guarantee that the handler gets unregistered when listener is released.
        /// </summary>
        [CanBeNull]
        private readonly TSource _source;

        /// <summary>
        /// Delegate to the method to call when the event fires.
        /// </summary>
        [NotNull]
        private readonly Action<TTarget, object, TEventArgs> _onEventAction;

        /// <summary>
        /// Delegate to the method to call when detaching from the event.
        /// </summary>
        [NotNull]
        private readonly Action<WeakEventListener<TTarget, TSource, TEventArgs>, TSource> _onDetachAction;

        /// <summary>
        /// Initializes a new instances of the WeakEventListener class that references the source but not the target.
        /// </summary>
        /// <param name="target">Instance subscribing to the event. The instance will not be referenced.</param>
        /// <param name="source">Instance providing the event. The instance will be referenced.</param>
        /// <param name="onEventAction">The static method to call when a event is received.</param>
        /// <param name="onAttachAction">The static action to attach to the event(s).</param>
        /// <param name="onDetachAction">The static action to detach from the event(s).</param>
        public WeakEventListener([NotNull] TTarget target, [NotNull] TSource source,
            [NotNull] Action<TTarget, object, TEventArgs> onEventAction,
            [NotNull] Action<WeakEventListener<TTarget, TSource, TEventArgs>, TSource> onAttachAction,
            [NotNull] Action<WeakEventListener<TTarget, TSource, TEventArgs>, TSource> onDetachAction)
        {
            if (!onEventAction.GetMethodInfo().IsStatic || !onAttachAction.GetMethodInfo().IsStatic || !onDetachAction.GetMethodInfo().IsStatic)
                throw new ArgumentException("Methods must be static, otherwise the event WeakEventListener class does not prevent memory leaks.");

            _weakTarget = new WeakReference<TTarget>(target);
            _source = source;
            _onEventAction = onEventAction;
            _onDetachAction = onDetachAction;

            onAttachAction(this, source);
        }

        /// <summary>
        /// Initializes a new instances of the WeakEventListener class that does not reference both source and target.
        /// </summary>
        /// <param name="target">Instance subscribing to the event. The instance will not be referenced.</param>
        /// <param name="source">Weak reference to the instance providing the event. When using this constructor the source will not be referenced, too.</param>
        /// <param name="onEventAction">The static method to call when a event is received.</param>
        /// <param name="onAttachAction">The static action to attach to the event(s).</param>
        /// <param name="onDetachAction">The static action to detach from the event(s).</param>
        public WeakEventListener([NotNull] TTarget target, [NotNull] WeakReference<TSource> source,
            [NotNull] Action<TTarget, object, TEventArgs> onEventAction,
            [NotNull] Action<WeakEventListener<TTarget, TSource, TEventArgs>, TSource> onAttachAction,
            [NotNull] Action<WeakEventListener<TTarget, TSource, TEventArgs>, TSource> onDetachAction)
        {
            if (!onEventAction.GetMethodInfo().IsStatic || !onAttachAction.GetMethodInfo().IsStatic || !onDetachAction.GetMethodInfo().IsStatic)
                throw new ArgumentException("Methods must be static, otherwise the event WeakEventListener class does not prevent memory leaks.");

            if (!source.TryGetTarget(out var sourceObject))
                throw new ArgumentException("Source object is already detached!");

            _weakTarget = new WeakReference<TTarget>(target);
            _weakSource = source;
            _onEventAction = onEventAction;
            _onDetachAction = onDetachAction;

            onAttachAction(this, sourceObject);
        }

        /// <summary>
        /// Handler for the subscribed event calls OnEventAction to handle it.
        /// </summary>
        /// <param name="source">Event source.</param>
        /// <param name="eventArgs">Event arguments.</param>
        public void OnEvent([NotNull] object source, [NotNull] TEventArgs eventArgs)
        {
            TTarget target;

            if (_weakTarget.TryGetTarget(out target))
            {
                // Call registered action
                _onEventAction(target, source, eventArgs);
            }
            else
            {
                // Detach from event
                Detach();
            }
        }

        /// <summary>
        /// Detaches from the subscribed event.
        /// </summary>
        public void Detach()
        {
            var source = _source;
            if (source == null)
            {
                _weakSource?.TryGetTarget(out source);
            }
            if (source == null)
                return;

            _onDetachAction(this, source);
        }
    }
}