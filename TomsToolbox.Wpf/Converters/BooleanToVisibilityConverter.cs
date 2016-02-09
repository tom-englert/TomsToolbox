namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using TomsToolbox.Core;

    /// <summary>
    /// The counterpart of VisibilityToBooleanConverter.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new BooleanToVisibilityConverter();

        /// <summary>
        /// The visibility value to be used when converting from a false boolean value. Defaults to Collapsed.
        /// </summary>
        public Visibility VisibilityWhenBooleanIsFalse { get; set; }

        /// <summary>
        /// The visibility value to be used when converting from a null boolean value. Defaults to Collapsed.
        /// </summary>
        public Visibility? VisibilityWhenBooleanIsNull { get; set; }

        /// <summary>
        /// The boolean value to be used when converting from a null Visibility value. Defaults to false.
        /// </summary>
        public bool? BooleanWhenVisibilityIsNull { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BooleanToVisibilityConverter() 
        {
            VisibilityWhenBooleanIsFalse = Visibility.Collapsed;
            VisibilityWhenBooleanIsNull = Visibility.Collapsed;
            BooleanWhenVisibilityIsNull = false;
        }

        /// <summary>
        /// Converts a value.
        /// True becomes Visible, false becomes the value set by VisibilityWhenBooleanIsFale, null becomes VisibilityWhenBooleanIsNull and UnSet is unchanged.
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
            if (value == null)
                return VisibilityWhenBooleanIsNull;

            if (value == DependencyProperty.UnsetValue)
            {
                value = false;
            }
            else if (!(value is bool))
            {
                PresentationTraceSources.DataBindingSource.TraceEvent(TraceEventType.Error, 9000, "{0} failed: {1}", GetType().Name, "Source is not a boolean.");
                value = false;
            }

            return (bool)value ? Visibility.Visible : VisibilityWhenBooleanIsFalse;
        }

        /// <summary>
        /// Converts a value.
        /// Visible becomes true, Collapsed and Hidden become false, null becomes BooleanWhenVisibilityIsNull and UnSet is unchanged.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == DependencyProperty.UnsetValue)
                return value;
            if (value == null)
                return BooleanWhenVisibilityIsNull;

            return (Visibility)value == Visibility.Visible;
        }

    }
}
