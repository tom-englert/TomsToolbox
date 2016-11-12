namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using JetBrains.Annotations;

    using TomsToolbox.Desktop;

    /// <summary>
    /// Converts WGS-84 coordinates (<see cref="Coordinates"/> ) into normalized logical XY coordinates (<see cref="Point"/>) in the range 0..1 and back.
    /// </summary>
    [ValueConversion(typeof(object), typeof(object))]
    public class CoordinatesToPointConverter : ValueConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new CoordinatesToPointConverter();

        /// <summary>
        /// Converts a value.
        /// Null and UnSet are unchanged.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// </returns>
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        /// <summary>
        /// Converts WGS-84 coordinates (<see cref="Coordinates" /> ) into normalized logical XY coordinates (<see cref="Point" />) in the range 0..1 and back.
        /// </summary>
        /// <param name="value">The <see cref="Coordinates" /> or <see cref="Point" /> value.</param>
        /// <returns>The <see cref="Coordinates" /> or <see cref="Point" /> value.</returns>
        /// <exception cref="System.InvalidOperationException">Value is neither a Point nor a Coordinates structure.</exception>
        [NotNull]
        public static object Convert(object value)
        {
            Contract.Ensures(Contract.Result<object>() != null);

            if (value is Point)
            {
                return (Coordinates)(Point)value;
            }

            if (value is Coordinates)
            {
                return (Point)(Coordinates)value;
            }

            throw new InvalidOperationException("Value is neither a Point nor a Coordinates structure");
        }
    }
}
