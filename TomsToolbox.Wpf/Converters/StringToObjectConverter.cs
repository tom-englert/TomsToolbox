namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// A <see cref="IValueConverter" /> wrapping a <see cref="TypeConverter" />
    /// </summary>
    [ValueConversion(typeof(string), typeof(object))]
    public class StringToObjectConverter : IValueConverter
    {
        private TypeConverter _typeConverter;

        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new StringToObjectConverter();

        /// <summary>
        /// Gets or sets the type of the type converter to use.
        /// If no type is specified, the type converter will be deduced form the target type.
        /// </summary>
        public Type TypeConverterType
        {
            get
            {
                if (_typeConverter != null)
                    return _typeConverter.GetType();
                return null;
            }
            set
            {
                if (value != null)
                {
                    if (typeof (TypeConverter).IsAssignableFrom(value) && (value.GetConstructor(Type.EmptyTypes) != null))
                    {
                        _typeConverter = (TypeConverter)Activator.CreateInstance(value);
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return value;

            var typeConverter = GetTypeConverter(targetType);
            if (typeConverter == null)
                return null;

            var text = value.ToString();
            if (string.IsNullOrEmpty(text))
                return null;

            try
            {
                return typeConverter.ConvertFromInvariantString(text);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} failed to convert '{1}': {2}", typeConverter, value, ex.Message));
            }
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return value;

            var typeConverter = GetTypeConverter(targetType);
            if (typeConverter == null)
                return null;

            try
            {
                return typeConverter.ConvertToInvariantString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} failed to convert '{1}': {2}", typeConverter, value, ex.Message), ex);
            }
        }

        private TypeConverter GetTypeConverter(Type targetType)
        {
            var typeConverter = _typeConverter;
            if (typeConverter != null)
                return typeConverter;

            return targetType == null ? null : TypeDescriptor.GetConverter(targetType);
        }
    }
}
