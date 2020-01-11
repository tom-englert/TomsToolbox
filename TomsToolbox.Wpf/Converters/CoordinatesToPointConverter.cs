namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using JetBrains.Annotations;

    /// <summary>
    /// Converts WGS-84 coordinates (<see cref="Coordinates"/> ) into normalized logical XY coordinates (<see cref="Point"/>) in the range 0..1 and back.
    /// </summary>
    [ValueConversion(typeof(object), typeof(object))]
    public class CoordinatesToPointConverter : ValueConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter Default = new CoordinatesToPointConverter();

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
        [NotNull]
        protected override object? Convert([CanBeNull] object? value, [CanBeNull] Type? targetType, [CanBeNull] object? parameter, [CanBeNull] CultureInfo? culture)
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
        [NotNull]
        protected override object? ConvertBack([CanBeNull] object? value, [CanBeNull] Type? targetType, [CanBeNull] object? parameter, [CanBeNull] CultureInfo? culture)
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
        public static object Convert([CanBeNull] object? value)
        {
            if (value is Point point)
            {
                return (Coordinates)point;
            }

            if (value is Coordinates coordinates)
            {
                return (Point)coordinates;
            }

            throw new InvalidOperationException("Value is neither a Point nor a Coordinates structure");
        }
    }
}
