namespace TomsToolbox.Wpf
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using JetBrains.Annotations;

    /// <summary>
    /// A panel that raises MouseDoubleClick events like the <see cref="Control"/>.
    /// </summary>
    public class DoubleClickPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleClickPanel"/> class.
        /// </summary>
        public DoubleClickPanel()
        {
            // Ensure we get click events by default.
            Background = Brushes.Transparent;
        }

        /// <summary>
        /// Raises the <see cref="Control.MouseDoubleClickEvent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseDoubleClick([NotNull] MouseButtonEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Raises the <see cref="Control.PreviewMouseDoubleClickEvent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPreviewMouseDoubleClick([NotNull] MouseButtonEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" /> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (HandleDoubleClick(e))
                return;

            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (HandleDoubleClick(e))
                return;

            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonDown" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the right mouse button was pressed.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (HandleDoubleClick(e))
                return;

            base.OnMouseRightButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseRightButtonDown" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the right mouse button was pressed.</param>
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (HandleDoubleClick(e))
                return;

            base.OnPreviewMouseRightButtonDown(e);
        }

        private bool HandleDoubleClick([NotNull] MouseButtonEventArgs e)
        {

            if (e.ClickCount != 2)
                return false;

            // ReSharper disable once AssignNullToNotNullAttribute
            var mouseButtonEventArg = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice);

            if ((e.RoutedEvent == PreviewMouseLeftButtonDownEvent) || (e.RoutedEvent == PreviewMouseRightButtonDownEvent))
            {
                mouseButtonEventArg.RoutedEvent = Control.PreviewMouseDoubleClickEvent;
                mouseButtonEventArg.Source = e.OriginalSource;
                OnPreviewMouseDoubleClick(mouseButtonEventArg);
            }
            else
            {
                mouseButtonEventArg.RoutedEvent = Control.MouseDoubleClickEvent;
                mouseButtonEventArg.Source = e.OriginalSource;
                OnMouseDoubleClick(mouseButtonEventArg);
            }

            if (mouseButtonEventArg.Handled)
            {
                e.Handled = true;
            }

            return true;
        }
    }
}
