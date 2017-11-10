namespace TomsToolbox.Wpf.XamlExtensions
{
    using System.Windows;

    using JetBrains.Annotations;

    /// <summary>
    /// Xaml extensions for the <see cref="System.Windows.Controls.Button"/>
    /// </summary>
    public static class Button
    {
        /// <summary>
        /// Gets the dialog result associated with the button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>The associated dialog result.</returns>
        [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.Button))]
        public static bool? GetDialogResult([NotNull] System.Windows.Controls.Button button)
        {
            return (bool?)button.GetValue(DialogResultProperty);
        }
        /// <summary>
        /// Sets the dialog result associated with the button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="value">The associated dialog result.</param>
        public static void SetDialogResult([NotNull] System.Windows.Controls.Button button, bool? value)
        {
            button.SetValue(DialogResultProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.XamlExtensions.DialogResult"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// Associates a dialog result with the button that it's attached to. 
        /// When the button is clicked, the <see cref="Window.DialogResult"/> of the Window is set.
        /// </summary>
        /// <remarks>
        /// This only has an effect if the window was created using the <see cref="Window.ShowDialog"/> method.
        /// </remarks>
        /// </AttachedPropertyComments>
        [NotNull]
        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached(
            "DialogResult", typeof(bool?), typeof(Button), new FrameworkPropertyMetadata(default(bool?), DialogResult_Changed));

        private static void DialogResult_Changed([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as System.Windows.Controls.Button;

            if (button == null)
                return;

            button.Click += Button_Click;
        }

        private static void Button_Click([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button == null)
                return;
            var window = Window.GetWindow(button);
            if (window == null)
                return;

            try
            {
                window.DialogResult = GetDialogResult(button); 
            }
            catch
            {
                // not started with "ShowDialog" or already closed.
            }
        }
    }
}
