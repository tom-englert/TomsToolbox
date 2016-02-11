namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// The counterpart of VisibilityToBooleanConverter.
    /// </summary>
    [ValueConversion(typeof(bool?), typeof(Visibility?))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new BooleanToVisibilityConverter();

        /// <summary>
        /// If the converter should swap Convert and ConvertBack operations.
        /// </summary>
        internal bool IsInverted { get; set; }

        /// <summary>
        /// The visibility value to be used when converting from a true bool value. Defaults to Visible.
        /// </summary>
        public Visibility? VisibilityWhenBooleanIsTrue { get; set; }

        /// <summary>
        /// The visibility value to be used when converting from a false bool value. Defaults to Collapsed.
        /// </summary>
        public Visibility? VisibilityWhenBooleanIsFalse { get; set; }

        /// <summary>
        /// The visibility value to be used when converting from a null bool value. Defaults to Collapsed.
        /// </summary>
        public Visibility? VisibilityWhenBooleanIsNull { get; set; }

        /// <summary>
        /// The bool value to be used when converting from a null Visibility value. Defaults to false.
        /// </summary>
        public bool? BooleanWhenVisibilityIsNull { get; set; }

        /// <summary>
        /// The bool value to be used when converting from a Visible Visibility value. Defaults to true.
        /// </summary>
        public bool? BooleanWhenVisibilityIsVisible { get; set; }

        /// <summary>
        /// The bool value to be used when converting from a Hidden Visibility value. Defaults to false.
        /// </summary>
        public bool? BooleanWhenVisibilityIsHidden { get; set; }

        /// <summary>
        /// The bool value to be used when converting from a Collapsed Visibility value. Defaults to false.
        /// </summary>
        public bool? BooleanWhenVisibilityIsCollapsed { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BooleanToVisibilityConverter() {
            VisibilityWhenBooleanIsTrue = Visibility.Visible;
            VisibilityWhenBooleanIsFalse = Visibility.Collapsed;
            VisibilityWhenBooleanIsNull = Visibility.Collapsed;
            BooleanWhenVisibilityIsNull = false;
            BooleanWhenVisibilityIsVisible = true;
            BooleanWhenVisibilityIsHidden = false;
            BooleanWhenVisibilityIsCollapsed = false;
        }

        internal object InternalConvert(object value, Type targetType, object parameter, CultureInfo culture, string methodName) {
            if (value == null)
                return VisibilityWhenBooleanIsNull;

            if (value == DependencyProperty.UnsetValue) {
                value = false;
            }
            else if (!(value is bool)) {
                this.TraceError("Source is not a boolean.", methodName);
                value = false;
            }

            return (bool)value ? VisibilityWhenBooleanIsTrue : VisibilityWhenBooleanIsFalse;
        }

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!IsInverted)
                return InternalConvert(value, targetType, parameter, culture, "Convert");

            return InternalConvertBack(value, targetType, parameter, culture, "Convert");
        }

        internal object InternalConvertBack(object value, Type targetType, object parameter, CultureInfo culture, string methodName) {
            if (value == DependencyProperty.UnsetValue)
                return value;
            if (value == null)
                return BooleanWhenVisibilityIsNull;

            try {
                switch ((Visibility)value) {
                    case Visibility.Visible:
                        return BooleanWhenVisibilityIsVisible;
                    case Visibility.Hidden:
                        return BooleanWhenVisibilityIsHidden;
                    case Visibility.Collapsed:
                        return BooleanWhenVisibilityIsCollapsed;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
            catch (Exception ex) {
                this.TraceError(ex.Message, methodName);
                return DependencyProperty.UnsetValue;
            }
        }

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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            if (!IsInverted)
                return InternalConvertBack(value, targetType, parameter, culture, "ConvertBack");

            return InternalConvert(value, targetType, parameter, culture, "ConvertBack");
        }
    }
}
