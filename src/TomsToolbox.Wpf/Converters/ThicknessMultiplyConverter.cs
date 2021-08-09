namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Multiplies all corresponding members of two <see cref="Thickness"/>. structures. 
    /// The first structure is passed as the converter value, the second as the converter parameter.
    /// </summary>
    [ValueConversion(typeof(Thickness), typeof(Thickness))]
    public class ThicknessMultiplyConverter : ValueConverter
    {
        private static readonly TypeConverter _typeConverter = new ThicknessConverter();

        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new ThicknessMultiplyConverter();


        /// <summary>
        /// Multiplies all corresponding members of both structures.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>The multiplied thickness.</returns>
        public static Thickness Multiply(Thickness first, Thickness second)
        {
            first.Left *= second.Left;
            first.Top *= second.Top;
            first.Right *= second.Right;
            first.Bottom *= second.Bottom;

            return first;
        }

        /// <summary>
        /// Converts a value. 
        /// Null or UnSet are unchanged, a Thickness is multiplied by the Thickness in the parameter.
        /// </summary>
        /// <returns>
        /// A converted value.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        protected override object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return Convert(value, parameter);
        }

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The multiplied thickness.</returns>
        public static object Convert(object? value, object? parameter)
        {
            var target = value.SafeCast<Thickness>();
            var factor = GetThicknessFromParameter(parameter);

            return Multiply(target, factor);
        }

        private static Thickness GetThicknessFromParameter(object? parameter)
        {
            if (parameter == null)
                return new Thickness(1.0);

            if (parameter is Thickness thickness)
                return thickness;

            if (parameter is string parameterString)
                return _typeConverter.ConvertFromInvariantString(parameterString).SafeCast<Thickness>();

            throw new ArgumentException("Invalid thickness parameter.", nameof(parameter));
        }
    }
}
