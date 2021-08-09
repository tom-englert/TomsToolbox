namespace TomsToolbox.Wpf.XamlExtensions
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// XAML helpers for keyboard navigation.
    /// </summary>
    public class KeyboardNavigation : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardNavigation"/> class.
        /// </summary>
        private KeyboardNavigation()
        {
            var inputManager = InputManager.Current;
            inputManager.PostProcessInput += InputManager_PostProcessInput;
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="KeyboardNavigation"/> class.
        /// </summary>
        public static KeyboardNavigation Current { get; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether the focus visual is visible on focused elements.
        /// </summary>
        /// <value>
        /// <c>true</c> if the focus visual is visible on focused elements; otherwise, <c>false</c>.
        /// </value>
        public bool IsFocusVisualVisible
        {
            get => this.GetValue<bool>(IsFocusVisualVisibleProperty);
            set => SetValue(IsFocusVisualVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsFocusVisualVisible"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsFocusVisualVisibleProperty =
            DependencyProperty.Register("IsFocusVisualVisible", typeof(bool), typeof(KeyboardNavigation));

        private void InputManager_PostProcessInput(object sender, ProcessInputEventArgs? e)
        {
            var inputManager = (InputManager)sender;
            IsFocusVisualVisible = SystemParameters.KeyboardCues || (inputManager.MostRecentInputDevice is KeyboardDevice);
        }
    }
}
