namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts a single number to a uniform <see cref="Thickness"/>, optionally multiplying with the thickness passed as converter parameter.
    /// </summary>
    [ValueConversion(typeof(double), typeof(Thickness))]
    public class DoubleToThicknessConverter : ValueConverter
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
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        protected override object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var thickness = ConvertNumberToThickness(value);

            return ThicknessMultiplyConverter.Convert(thickness, parameter);
        }

        private static Thickness ConvertNumberToThickness(object? value)
        {
            var length = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            var thickness = new Thickness(length);
            return thickness;
        }
    }
}
