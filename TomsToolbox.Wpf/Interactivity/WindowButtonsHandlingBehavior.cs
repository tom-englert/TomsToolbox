namespace TomsToolbox.Wpf.Interactivity
{
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using TomsToolbox.Wpf;

    /// <summary>
    /// Attaches default handling for the <see cref="WindowCommands"/>
    /// </summary>
    public class WindowButtonsHandlingBehavior : Behavior<Window>
    {
        /// <summary>
        /// Gets or sets the window style of the window. 
        /// Since the style of the associated window is usually <see cref="System.Windows.WindowStyle.None"/> you have to set the intended style here.
        /// </summary>
        public WindowStyle WindowStyle
        {
            get;
            set;
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

            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.CommandBindings.Add(new CommandBinding(WindowCommands.Close, Close));

            if (WindowStyle == WindowStyle.ToolWindow)
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

            e.CanExecute = window.WindowState == WindowState.Normal;
        }

        private void Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.WindowState = WindowState.Maximized;
        }
    }
}
