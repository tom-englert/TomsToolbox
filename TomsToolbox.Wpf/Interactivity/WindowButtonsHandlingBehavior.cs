namespace TomsToolbox.Wpf.Interactivity
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    using JetBrains.Annotations;

    /// <summary>
    /// Attaches default handling for the <see cref="WindowCommands"/>
    /// </summary>
    public class WindowButtonsHandlingBehavior : Behavior<DependencyObject>
    {
        [CanBeNull]
        private Window _window;
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            var window = _window = Window.GetWindow(AssociatedObject);
            if (window == null)
                return;

            window.CommandBindings.Add(new CommandBinding(WindowCommands.Close, Close));

            if (window.ResizeMode == ResizeMode.NoResize)
                return;

            // to ensure min/max/restore buttons are updated correctly.
            window.StateChanged += (_, __) => CommandManager.InvalidateRequerySuggested();

            window.CommandBindings.Add(new CommandBinding(WindowCommands.Minimize, Minimize));
            window.CommandBindings.Add(new CommandBinding(WindowCommands.Maximize, Maximize, CanMaximize));
            window.CommandBindings.Add(new CommandBinding(WindowCommands.Restore, Restore, CanRestore));
        }

        private void Minimize([CanBeNull] object sender, [CanBeNull] ExecutedRoutedEventArgs e)
        {
            var window = _window;
            if (window == null)
                return;

            window.WindowState = WindowState.Minimized;
        }

        private void Close([CanBeNull] object sender, [CanBeNull] ExecutedRoutedEventArgs e)
        {
            _window?.Close();
        }

        private void CanRestore([CanBeNull] object sender, [NotNull] CanExecuteRoutedEventArgs e)
        {
            var window = _window;
            if (window == null)
                return;

            e.CanExecute = window.WindowState != WindowState.Normal;
        }

        private void Restore([CanBeNull] object sender, [CanBeNull] ExecutedRoutedEventArgs e)
        {
            var window = _window;
            if (window == null)
                return;

            window.WindowState = WindowState.Normal;
        }

        private void CanMaximize([CanBeNull] object sender, [NotNull] CanExecuteRoutedEventArgs e)
        {
            var window = _window;
            if (window == null)
                return;

            e.CanExecute = (window.WindowState == WindowState.Normal) && ((window.ResizeMode == ResizeMode.CanResize) || (window.ResizeMode == ResizeMode.CanResizeWithGrip));
        }

        private void Maximize([CanBeNull] object sender, [CanBeNull] ExecutedRoutedEventArgs e)
        {
            var window = _window;
            if (window == null)
                return;

            window.WindowState = WindowState.Maximized;
        }
    }
}
