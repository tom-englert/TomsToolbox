namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Converts an <see cref="ImageSource"/> into an <see cref="Image"/>. 
    /// Needed to assign an image source to an item via a style setter, e.g. <see cref="MenuItem.Icon"/>.
    /// </summary>
    [ValueConversion(typeof(ImageSource), typeof(ImageSource))]
    public class ImageSourceToImageConverter : IValueConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new ImageSourceToImageConverter();

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return value;

            try
            {
                return new Image { Source = (ImageSource)value };
            }
            catch (Exception ex)
            {
                PresentationTraceSources.DataBindingSource.TraceEvent(TraceEventType.Error, 9000, "{0} failed: {1}", GetType().Name, ex.Message);
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
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
