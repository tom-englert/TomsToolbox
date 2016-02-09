namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts a single number to a uniform <see cref="Thickness"/>, optionally multiplying with the thickness passed as converter parameter.
    /// </summary>
    [ValueConversion(typeof(double), typeof(Thickness))]
    public class DoubleToThicknessConverter : IValueConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new DoubleToThicknessConverter();

        /// <summary>
        /// Converts a value. 
        /// Null and UnSet are unchanged.
        /// </summary>
        /// <returns>
        /// A converted value.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return value;

            try
            {
                var thickness = ConvertNumberToThickness(value);

                return ThicknessMultiplyConverter.Default.Convert(thickness, targetType, parameter, culture);
            }
            catch (Exception ex)
            {
                PresentationTraceSources.DataBindingSource.TraceEvent(TraceEventType.Error, 9000, "{0} failed: {1}", GetType().Name, ex.Message);
                return DependencyProperty.UnsetValue;
            }
        }

        private static Thickness ConvertNumberToThickness(object value)
        {
            // let it fail fast so we are not left wondering what went wrong
            var length = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            var thickness = new Thickness(length);
            return thickness;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
