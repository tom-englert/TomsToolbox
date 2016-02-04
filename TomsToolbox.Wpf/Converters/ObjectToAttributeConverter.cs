namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Data;

    using TomsToolbox.Desktop;

    /// <summary>
    /// Converts an object to a value derived from an attribute of the object.
    /// </summary>
    /// <typeparam name="T">The attribute to look up.</typeparam>
    public abstract class ObjectToAttributeConverter<T> : IValueConverter where T : Attribute
    {
        /// <summary>
        /// Does the conversion.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="enumType">An optional type of an enum to support converting <see cref="Enum"/> where the value is given as a number or string.</param>
        /// <param name="selector">The selector to get the desired value from the attribute.</param>
        /// <returns>The converted value.</returns>
        protected static string InternalConvert(object value, Type enumType, Func<T, string> selector)
        {
            Contract.Requires(value != null);
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return InternalConvert(value, enumType, selector, _ => true);
        }

        /// <summary>
        /// Does the conversion.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="enumType">An optional type of an enum to support converting <see cref="Enum"/> where the value is given as a number or string.</param>
        /// <param name="selector">The selector to get the desired value from the attribute.</param>
        /// <param name="predicate">A predicate to search for a specific attribute.</param>
        /// <returns>
        /// The converted value.
        /// </returns>
        protected static string InternalConvert(object value, Type enumType, Func<T, string> selector, Func<T, bool> predicate)
        {
            Contract.Requires(value != null);
            Contract.Requires(selector != null);
            Contract.Requires(predicate != null);
            Contract.Ensures(Contract.Result<string>() != null);

            var valueType = value.GetType();
            var valueString = value.ToString();

            ICustomAttributeProvider fieldInfo;
            if (valueType.IsEnum)
            {
                fieldInfo = valueType.GetField(valueString);
            }
            else if (enumType != null)
            {
                if (!enumType.IsEnum)
                    throw new ArgumentException(@"Parameter must be an enum type or null.", "enumType");

                var enumName = valueType == typeof(string) ? (string)value : Enum.ToObject(enumType, value).ToString();
                fieldInfo = enumType.GetField(enumName);
            }
            else
            {
                fieldInfo = valueType;
            }

            if (fieldInfo != null)
            {
                return fieldInfo.GetCustomAttributes<T>(false)
                    .Where(predicate)
                    .Select(selector)
                    .FirstOrDefault() ?? valueString;
            }

            return valueString;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
