namespace TomsToolbox.Wpf.Interactivity
{
    using System.Windows;
    using System.Windows.Interactivity;

    using JetBrains.Annotations;

    /// <summary>
    /// A <see cref="Behavior{T}"/> with build in support for the <see cref="FrameworkElement.Loaded"/> and <see cref="FrameworkElement.Unloaded"/> events.
    /// </summary>
    /// <typeparam name="T">The type the <see cref="FrameworkElementBehavior{T}"/> can be attached to.</typeparam>
    public class FrameworkElementBehavior<T> : Behavior<T>
        where T : FrameworkElement
    {
        /// <summary>
        /// Gets a value indicating whether the associated object is loaded.
        /// </summary>
        protected bool IsLoaded => AssociatedObject?.IsLoaded ?? false;

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected virtual void OnAssociatedObjectLoaded()
        {
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected virtual void OnAssociatedObjectUnloaded()
        {
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

            var associatedObject = AssociatedObject;

            associatedObject.Loaded += AssociatedObject_Loaded;
            associatedObject.Unloaded += AssociatedObject_Unloaded;
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

            var associatedObject = AssociatedObject;

            associatedObject.Loaded -= AssociatedObject_Loaded;
            associatedObject.Unloaded -= AssociatedObject_Unloaded;
        }

        private void AssociatedObject_Loaded([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            OnAssociatedObjectLoaded();
        }

        private void AssociatedObject_Unloaded([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            OnAssociatedObjectUnloaded();
        }
    }
}
