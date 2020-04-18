namespace TomsToolbox.Wpf.Interactivity
{
    using System.Windows;
    using System.Windows.Controls;

    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// A behavior to enable access to the <see cref="PasswordBox.Password"/> property.
    /// </summary>
    /// <remarks>
    /// Not that accessing the password of the password box via binding has some security issues.
    /// </remarks>
    public class PasswordBoxBindingBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// Gets or sets the password as plain text.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(PasswordBoxBindingBehavior), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Text_Changed));

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
            AssociatedObject.Password = Text;
            AssociatedObject.SelectAll();
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }

        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Text = AssociatedObject.Password;
        }

        private static void Text_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PasswordBoxBindingBehavior)d).Text_Changed((string)e.NewValue);
        }

        private void Text_Changed(string newValue)
        {
            if (AssociatedObject != null && AssociatedObject.Password != newValue)
            {
                AssociatedObject.Password = newValue;
                AssociatedObject.SelectAll();
            }
        }
    }
}
