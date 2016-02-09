namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Multiplies all corresponding members of two <see cref="Thickness"/>. structures. 
    /// The first structure is passed as the converter value, the second as the converter parameter.
    /// </summary>
    [ValueConversion(typeof(Thickness), typeof(Thickness))]
    public class ThicknessMultiplyConverter : IValueConverter
    {
        private static readonly TypeConverter _typeConverter = new ThicknessConverter();

        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new ThicknessMultiplyConverter();


        /// <summary>
        /// Multiplies all corresponding members of both structures.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>The multiplied thickness.</returns>
        public static Thickness Multiply(Thickness first, Thickness second)
        {
            first.Left *= second.Left;
            first.Top *= second.Top;
            first.Right *= second.Right;
            first.Bottom *= second.Bottom;

            return first;
        }

        /// <summary>
        /// Converts a value. 
        /// Null or UnSet are unchanged, a Thickness is multiplied by the Thickness in the parameter.
        /// </summary>
        /// <returns>
        /// A converted value.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return value;

            var target = (Thickness)value;
            Thickness factor;
            if (parameter == null)
            {
                factor = new Thickness(1.0);
            }
            else if (parameter is Thickness)
            {
                factor = (Thickness) parameter;
            }
            else if (parameter is string)
            {
                factor = (Thickness) _typeConverter.ConvertFromInvariantString((string) parameter);
            }
            else
            {
                throw new ArgumentException("Invalid thickness parameter.", "parameter");
            }

            return Multiply(target, factor);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
