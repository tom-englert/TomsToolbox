namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts a single number to a uniform <see cref="Thickness"/>, optionally multiplying with the thickness passed as converter parameter.
    /// </summary>
    public class DoubleToThicknessConverter : IValueConverter
    {
        private static readonly TypeConverter _typeConverter = new ThicknessConverter();

        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new DoubleToThicknessConverter();

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var thickness = ConvertNumberToThickness(value);

            try
            {
                var factor = (parameter as Thickness?) ?? (_typeConverter.ConvertFromInvariantString(parameter as string) as Thickness?);

                return ThicknessMultiplyConverter.Multiply(thickness, factor.GetValueOrDefault());
            }
            catch (SystemException)
            {
            }

            return thickness;
        }

        private static Thickness ConvertNumberToThickness(object value)
        {
            try
            {
                var length = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                var thickness = new Thickness(length);
                return thickness;
            }
            catch (SystemException)
            {
            }

            return new Thickness();
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
            throw new NotImplementedException();
        }
    }
}
