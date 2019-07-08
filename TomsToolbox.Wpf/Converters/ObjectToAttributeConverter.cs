namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Converts an object to a value derived from an attribute of the object.
    /// </summary>
    /// <typeparam name="T">The attribute to look up.</typeparam>
    public abstract class ObjectToAttributeConverter<T> : ValueConverter where T : Attribute
    {
        /// <summary>
        /// Does the conversion.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="enumType">An optional type of an enum to support converting <see cref="Enum"/> where the value is given as a number or string.</param>
        /// <param name="selector">The selector to get the desired value from the attribute.</param>
        /// <returns>The converted value.</returns>
        [CanBeNull]
        protected static string InternalConvert([CanBeNull] object value, [CanBeNull] Type enumType, [NotNull] Func<T, string> selector)
        {
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
        [CanBeNull]
        protected static string InternalConvert([CanBeNull] object value, [CanBeNull] Type enumType, [NotNull] Func<T, string> selector, [NotNull] Func<T, bool> predicate)
        {
            if (value == null)
                // ReSharper disable once AssignNullToNotNullAttribute
                return null;

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
                    throw new ArgumentException(@"Parameter must be an enum type or null.", nameof(enumType));

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
        [CanBeNull]
        protected abstract override object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture);
    }
}
