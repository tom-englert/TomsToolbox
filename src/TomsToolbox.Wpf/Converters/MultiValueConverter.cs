namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// A base class for multi-value converters performing pre-check of value and error handling.
    /// </summary>
    public abstract class MultiValueConverter : MarkupExtension, IMultiValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="values">The values produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        protected abstract object? Convert(object?[] values, Type? targetType, object? parameter, CultureInfo? culture);

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetTypes">The types to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="InvalidOperationException">ConvertBack is not supported by this converter.</exception>
        protected virtual object?[] ConvertBack(object value, Type?[]? targetTypes, object? parameter, CultureInfo? culture)
        {
            throw new InvalidOperationException("ConvertBack is not supported by this converter.");
        }

        object? IMultiValueConverter.Convert(object?[]? values, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (values == null)
                return null;
            if (values.Any(x => x == null))
                return null;
            if (values.Any(x => x == DependencyProperty.UnsetValue))
                return DependencyProperty.UnsetValue;

            try
            {
                return Convert(values, targetType, parameter, culture);
            }
            catch (Exception ex)
            {
                this.TraceError(ex.Message, "Convert");
                return DependencyProperty.UnsetValue;
            }
        }

        object?[]? IMultiValueConverter.ConvertBack(object? value, Type[]? targetTypes, object? parameter, CultureInfo? culture)
        {
            if (value == null)
                return null;

            try
            {
                return ConvertBack(value, targetTypes, parameter, culture);
            }
            catch (Exception ex)
            {
                this.TraceError(ex.Message, "ConvertBack");
                return null;
            }
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
