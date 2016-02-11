﻿namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Converts a color name to the corresponding solid color brush. See <see cref="BrushConverter"/> for supported values.
    /// </summary>
    [ValueConversion(typeof(string), typeof(Brush))]
    public class ColorNameToBrushConverter : ValueConverter
    {
        private static readonly TypeConverter _typeConverter = new BrushConverter();

        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new ColorNameToBrushConverter();

        /// <summary>
        /// Converts the specified color name.
        /// Null and UnSet are unchanged.
        /// </summary>
        /// <param name="colorName">The color name.</param>
        /// <returns>The corresponding brush if the conversion was successful; otherwise <c>null</c>.</returns>
        public static Brush Convert(string colorName)
        {
            // let it fail fast so we are not left wondering what went wrong
            return colorName != null ? _typeConverter.ConvertFromInvariantString(colorName) as Brush : null;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((string)value);
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_typeConverter != null);
        }
    }
}
