namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts the specified enum-type into an array of the individual enum values.
    /// The converter parameter can be used to specify a comma separated exclude list.
    /// </summary>
    [ValueConversion(typeof(Type), typeof(Array))]
    public class EnumToValuesConverter : IValueConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new EnumToValuesConverter();

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

            return Convert((Type)value, (string)parameter);
        }

        /// <summary>
        /// Converts the specified enum-type into an array of the individual enum values.
        /// </summary>
        /// <param name="type">The enum type.</param>
        /// <param name="excluded">A comma separated list of values to exclude.</param>
        /// <returns>An array of the enum's values.</returns>
        public static Array Convert(Type type, string excluded = null)
        {
            if (type == null)
                return null;

            var values = Enum.GetValues(type);

            if (excluded == null)
                return values;

            var typeConverter = TypeDescriptor.GetConverter(type);
            var excludeList = excluded.Split(',').Select(typeConverter.ConvertFromInvariantString);
            var filtered = values.OfType<object>().Except(excludeList).ToArray();

            return new ArrayList(filtered).ToArray(type);
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
