namespace TomsToolbox.Wpf.Interactivity
{
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// Attaches default handling for the <see cref="WindowCommands"/>
    /// </summary>
    public class WindowButtonsHandlingBehavior : Behavior<Window>
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.CommandBindings.Add(new CommandBinding(WindowCommands.Close, Close));

            if (window.ResizeMode == ResizeMode.NoResize)
                return;

            window.CommandBindings.Add(new CommandBinding(WindowCommands.Minimize, Minimize));
            window.CommandBindings.Add(new CommandBinding(WindowCommands.Maximize, Maximize, CanMaximize));
            window.CommandBindings.Add(new CommandBinding(WindowCommands.Restore, Restore, CanRestore));
        }

        private void Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.WindowState = WindowState.Minimized;
        }

        private void Close(object sender, ExecutedRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.Close();
        }

        private void CanRestore(object sender, CanExecuteRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            e.CanExecute = window.WindowState != WindowState.Normal;
        }

        private void Restore(object sender, ExecutedRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.WindowState = WindowState.Normal;
        }

        private void CanMaximize(object sender, CanExecuteRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            e.CanExecute = (window.WindowState == WindowState.Normal) && ((window.ResizeMode == ResizeMode.CanResize) || (window.ResizeMode == ResizeMode.CanResizeWithGrip));
        }

        private void Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.WindowState = WindowState.Maximized;
        }
    }
}
