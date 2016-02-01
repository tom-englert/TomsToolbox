namespace TomsToolbox.Wpf.XamlExtensions
{
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Input;

    using TomsToolbox.Desktop;

    /// <summary>
    /// XAML helpers for keyboard navigation.
    /// </summary>
    public class KeyboardNavigation : DependencyObject
    {
        private static readonly KeyboardNavigation _current = new KeyboardNavigation();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardNavigation"/> class.
        /// </summary>
        private KeyboardNavigation()
        {
            var inputManager = InputManager.Current;
            Contract.Assume(inputManager != null); // TODO: remove after fixing missing contract in CC 
            inputManager.PostProcessInput += InputManager_PostProcessInput;
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="KeyboardNavigation"/> class.
        /// </summary>
        public static KeyboardNavigation Current
        {
            get
            {
                Contract.Ensures(Contract.Result<KeyboardNavigation>() != null);
                return _current;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the focus visual is visible on focused elements.
        /// </summary>
        /// <value>
        /// <c>true</c> if the focus visual is visible on focused elements; otherwise, <c>false</c>.
        /// </value>
        public bool IsFocusVisualVisible
        {
            get { return this.GetValue<bool>(IsFocusVisualVisibleProperty); }
            set { SetValue(IsFocusVisualVisibleProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="IsFocusVisualVisible"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsFocusVisualVisibleProperty =
            DependencyProperty.Register("IsFocusVisualVisible", typeof(bool), typeof(KeyboardNavigation));

        private void InputManager_PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            var inputManager = (InputManager)sender;
            Contract.Assume(inputManager != null);
            IsFocusVisualVisible = SystemParameters.KeyboardCues || (inputManager.MostRecentInputDevice is KeyboardDevice);
        }
    }
}
