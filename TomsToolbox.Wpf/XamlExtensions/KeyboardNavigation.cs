namespace TomsToolbox.Wpf.XamlExtensions
{
    using System.Windows;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using TomsToolbox.Desktop;

    /// <summary>
    /// XAML helpers for keyboard navigation.
    /// </summary>
    public class KeyboardNavigation : DependencyObject
    {
        [NotNull] private static readonly KeyboardNavigation _current = new KeyboardNavigation();

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
        [NotNull]
        public static KeyboardNavigation Current
        {
            get
            {
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
            get => this.GetValue<bool>(IsFocusVisualVisibleProperty);
            set => SetValue(IsFocusVisualVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsFocusVisualVisible"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty IsFocusVisualVisibleProperty =
            DependencyProperty.Register("IsFocusVisualVisible", typeof(bool), typeof(KeyboardNavigation));

        private void InputManager_PostProcessInput([NotNull] object sender, [CanBeNull] ProcessInputEventArgs e)
        {
            var inputManager = (InputManager)sender;
            IsFocusVisualVisible = SystemParameters.KeyboardCues || (inputManager.MostRecentInputDevice is KeyboardDevice);
        }
    }
}
