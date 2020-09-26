namespace TomsToolbox.Wpf.XamlExtensions
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    using JetBrains.Annotations;

    /// <summary>
    /// Validation XAML extensions.
    /// </summary>
    public static class Validation
    {
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        private static readonly DependencyPropertyDescriptor _propertyDescriptor = DependencyPropertyDescriptor.FromProperty(System.Windows.Controls.Validation.HasErrorProperty, typeof(FrameworkElement));

        /// <summary>
        /// Gets whether validation errors are shown in the tool tip of the associated control or not.
        /// </summary>
        /// <param name="obj">The target control.</param>
        /// <returns><c>true</c>if validation errors are shown in the tool tip of the associated control; otherwise <c>false</c>.</returns>
        public static bool GetShowErrorInTooltip([NotNull] DependencyObject obj)
        {
            return ((bool?)obj.GetValue(ShowErrorInTooltipProperty)).GetValueOrDefault();
        }
        /// <summary>
        /// Sets whether validation errors are shown in the tool tip of the associated control or not.
        /// </summary>
        /// <param name="obj">The target control.</param>
        /// <param name="value">if set to <c>true</c> validation errors are shown in the tool tip of the associated control.</param>
        public static void SetShowErrorInTooltip([NotNull] DependencyObject obj, bool value)
        {
            obj.SetValue(ShowErrorInTooltipProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.XamlExtensions.Validation.ShowErrorInTooltip"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// If set to <c>true</c> validation errors are shown in the tooltip of the associated control.
        /// </summary>
        /// <remarks>
        /// Use this as a replacement for <see cref="ResourceKeys.ControlWithValidationErrorToolTipStyle"/> where a neutral base style is not applicable.
        /// </remarks>
        /// </AttachedPropertyComments>
        [NotNull]
        public static readonly DependencyProperty ShowErrorInTooltipProperty =
            DependencyProperty.RegisterAttached("ShowErrorInTooltip", typeof(bool), typeof(Validation), new FrameworkPropertyMetadata(false, ShowErrorInTooltip_Changed));

        private static void ShowErrorInTooltip_Changed([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (true.Equals(e.NewValue))
            {
                _propertyDescriptor.AddValueChanged(d, ValidationHasErrors_Changed);
            }
            else
            {
                _propertyDescriptor.RemoveValueChanged(d, ValidationHasErrors_Changed);
            }
        }

        private static void ValidationHasErrors_Changed(object? sender, EventArgs e)
        {
            var target = sender as FrameworkElement;
            if (target == null)
                return;

            var hasError = System.Windows.Controls.Validation.GetHasError(target);
            if (!hasError)
            {
                target.ToolTip = null;
                return;
            }

            var errors = System.Windows.Controls.Validation.GetErrors(target)?.Select(err => err?.ErrorContent?.ToString()).Where(err => err != null);
            var toolTip = string.Join("\r\n", errors ?? Enumerable.Empty<string>());
            target.ToolTip = toolTip;
        }
    }
}
