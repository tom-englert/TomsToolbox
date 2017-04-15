namespace TomsToolbox.Wpf.Interactivity
{
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    using JetBrains.Annotations;

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

        private void Minimize([CanBeNull] object sender, [CanBeNull] ExecutedRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.WindowState = WindowState.Minimized;
        }

        private void Close([CanBeNull] object sender, [CanBeNull] ExecutedRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.Close();
        }

        private void CanRestore([CanBeNull] object sender, [NotNull] CanExecuteRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            e.CanExecute = window.WindowState != WindowState.Normal;
        }

        private void Restore([CanBeNull] object sender, [CanBeNull] ExecutedRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.WindowState = WindowState.Normal;
        }

        private void CanMaximize([CanBeNull] object sender, [NotNull] CanExecuteRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            e.CanExecute = (window.WindowState == WindowState.Normal) && ((window.ResizeMode == ResizeMode.CanResize) || (window.ResizeMode == ResizeMode.CanResizeWithGrip));
        }

        private void Maximize([CanBeNull] object sender, [CanBeNull] ExecutedRoutedEventArgs e)
        {
            var window = AssociatedObject;
            Contract.Assume(window != null);

            window.WindowState = WindowState.Maximized;
        }
    }
}
