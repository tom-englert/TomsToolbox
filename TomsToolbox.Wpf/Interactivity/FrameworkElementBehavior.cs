namespace TomsToolbox.Wpf.Interactivity
{
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Interactivity;

    /// <summary>
    /// A <see cref="Behavior{T}"/> with build in support for the <see cref="FrameworkElement.Loaded"/> and <see cref="FrameworkElement.Unloaded"/> events.
    /// </summary>
    /// <typeparam name="T">The type the <see cref="FrameworkElementBehavior{T}"/> can be attached to.</typeparam>
    public class FrameworkElementBehavior<T> : Behavior<T>
        where T : FrameworkElement
    {
        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected virtual void OnAssociatedObjectLoaded()
        {
            Contract.Requires(AssociatedObject != null);
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected virtual void OnAssociatedObjectUnloaded()
        {
            Contract.Requires(AssociatedObject != null);
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            Contract.Assume(AssociatedObject != null);

            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;

            Contract.Assume(!AssociatedObject.IsLoaded);
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            Contract.Assume(AssociatedObject != null);

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            Contract.Assume(AssociatedObject != null);
            OnAssociatedObjectLoaded();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            Contract.Assume(AssociatedObject != null);
            OnAssociatedObjectUnloaded();
        }
    }
}
