namespace TomsToolbox.Wpf.XamlExtensions;

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using TomsToolbox.Essentials;

/// <summary>
/// Validation XAML extensions.
/// </summary>
public static class Validation
{
    private static readonly DependencyPropertyDescriptor _propertyDescriptor = DependencyPropertyDescriptor.FromProperty(System.Windows.Controls.Validation.HasErrorProperty, typeof(FrameworkElement));

    /// <summary>
    /// Gets whether validation errors are shown in the tool tip of the associated control or not.
    /// </summary>
    /// <param name="obj">The target control.</param>
    /// <returns><c>true</c>if validation errors are shown in the tool tip of the associated control; otherwise <c>false</c>.</returns>
    public static bool GetShowErrorInTooltip(DependencyObject obj)
    {
        return ((bool?)obj.GetValue(ShowErrorInTooltipProperty)).GetValueOrDefault();
    }
    /// <summary>
    /// Sets whether validation errors are shown in the tool tip of the associated control or not.
    /// </summary>
    /// <param name="obj">The target control.</param>
    /// <param name="value">if set to <c>true</c> validation errors are shown in the tool tip of the associated control.</param>
    public static void SetShowErrorInTooltip(DependencyObject obj, bool value)
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
    public static readonly DependencyProperty ShowErrorInTooltipProperty =
        DependencyProperty.RegisterAttached("ShowErrorInTooltip", typeof(bool), typeof(Validation), new FrameworkPropertyMetadata(false, ShowErrorInTooltip_Changed));

    private static void ShowErrorInTooltip_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement target)
            return;

        static void OnLoaded(object? sender, EventArgs _)
        {
            if (sender is not FrameworkElement target)
                return;

            _propertyDescriptor.RemoveValueChanged(target, ValidationHasErrors_Changed);
            _propertyDescriptor.AddValueChanged(target, ValidationHasErrors_Changed);

            ValidationHasErrors_Changed(sender, EventArgs.Empty);
        }

        static void OnUnloaded(object? sender, EventArgs _)
        {
            if (sender is not FrameworkElement target)
                return;

            _propertyDescriptor.RemoveValueChanged(target, ValidationHasErrors_Changed);
        }

        if (true.Equals(e.NewValue))
        {
            target.Loaded += OnLoaded;
            target.Unloaded += OnUnloaded;
            if (target.IsLoaded)
            {
                OnLoaded(target, EventArgs.Empty);
            }
        }
        else
        {
            target.Loaded -= OnLoaded;
            target.Unloaded -= OnUnloaded;

            _propertyDescriptor.RemoveValueChanged(target, ValidationHasErrors_Changed);
        }
    }

    private static void ValidationHasErrors_Changed(object? sender, EventArgs _)
    {
        if (sender is not FrameworkElement target)
            return;

        var hasError = System.Windows.Controls.Validation.GetHasError(target);
        if (!hasError)
        {
            target.ToolTip = null;
            return;
        }

        var errors = System.Windows.Controls.Validation
            .GetErrors(target)
            ?.Select(err => err?.ErrorContent?.ToString())
            .ExceptNullItems();
        var toolTip = string.Join("\r\n", errors ?? Enumerable.Empty<string>());
        target.ToolTip = toolTip;
    }
}
