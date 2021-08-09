namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Converts an <see cref="ImageSource"/> into an <see cref="Image"/>. 
    /// Needed to assign an image source to an item via a style setter, e.g. <see cref="MenuItem.Icon"/>.
    /// </summary>
    [ValueConversion(typeof(ImageSource), typeof(ImageSource))]
    public class ImageSourceToImageConverter : ValueConverter
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
        protected override object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return new Image { Source = value as ImageSource };
        }
    }
}
