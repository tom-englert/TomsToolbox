namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Data;

    using TomsToolbox.Essentials;

    /// <summary>
    /// A <see cref="IValueConverter" /> wrapping a <see cref="TypeConverter" />
    /// </summary>
    [ValueConversion(typeof(string), typeof(object))]
    public class StringToObjectConverter : ValueConverter
    {
        private TypeConverter? _typeConverter;

        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new StringToObjectConverter();

        /// <summary>
        /// Gets or sets the type of the type converter to use.
        /// If no type is specified, the type converter will be deduced form the target type.
        /// </summary>
        public Type? TypeConverterType
        {
            get => _typeConverter?.GetType();
            set
            {
                if (value != null)
                {
                    if (typeof(TypeConverter).IsAssignableFrom(value) && (value.GetConstructor(Type.EmptyTypes) != null))
                    {
                        _typeConverter = Activator.CreateInstance(value) as TypeConverter;
                        return;
                    }

                    Trace.TraceError("{0} is not a valid type converter type", value);
                }

                _typeConverter = null;
            }
        }

        /// <summary>
        /// Converts a value. Null or UnSet are unchanged.
        /// </summary>
        /// <returns>
        /// A converted value.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        protected override object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var typeConverter = GetTypeConverter(targetType);
            if (typeConverter == null)
                return null;

            var text = value?.ToString();
            if (text.IsNullOrEmpty())
                return null;

            return typeConverter.ConvertFromInvariantString(text);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        protected override object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var typeConverter = GetTypeConverter(targetType);

            return typeConverter?.ConvertToInvariantString(value ?? string.Empty);
        }

        private TypeConverter? GetTypeConverter(Type? targetType)
        {
            var typeConverter = _typeConverter;
            if (typeConverter != null)
                return typeConverter;

            return targetType == null ? null : TypeDescriptor.GetConverter(targetType);
        }
    }
}
