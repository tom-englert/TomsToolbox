namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    using JetBrains.Annotations;

    /// <summary>
    /// A base class for mutli value converters performing pre-check of value and error handling.
    /// </summary>
    public abstract class MultiValueConverter : IMultiValueConverter
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
        [CanBeNull]
        protected abstract object Convert([NotNull, ItemCanBeNull] object[] values, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture);

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
        [CanBeNull, ItemCanBeNull]
        protected virtual object[] ConvertBack([NotNull] object value, [CanBeNull, ItemCanBeNull] Type[] targetTypes, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
        {
            throw new InvalidOperationException("ConvertBack is not supported by this converter.");
        }

        [CanBeNull]
        object IMultiValueConverter.Convert([CanBeNull] object[] values, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
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

        [CanBeNull]
        object[] IMultiValueConverter.ConvertBack([CanBeNull] object value, [CanBeNull] Type[] targetTypes, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
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
    }
}
