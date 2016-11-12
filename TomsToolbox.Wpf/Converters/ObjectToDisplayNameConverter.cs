namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Windows.Data;

    using JetBrains.Annotations;

    /// <summary>
    /// Takes an object and returns the display name taken from it's <see cref="DisplayNameAttribute"/>
    /// </summary>
    /// <example><code language="C#">
    /// enum Items
    /// {
    ///     [DisplayName("This is item 1")]
    ///     Item1,
    ///     [DisplayName("This is item 2")]
    ///     Item2
    /// }
    ///
    /// Assert.Equals("This is item 1", ObjectToDisplayNameConverter.Convert(Items.Item1));
    /// </code></example>
    /// <remarks>Works with any object; for enum types the attribute of the field is returned.</remarks>
    [ValueConversion(typeof(object), typeof(string))]
    public class ObjectToDisplayNameConverter : ObjectToAttributeConverter<DisplayNameAttribute>
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new ObjectToDisplayNameConverter();

        /// <summary>
        /// Converts a value.
        /// UnSet is unchanged, null becomes an empty string.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// </returns>
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, parameter as Type);
        }

        /// <summary>
        /// Converts the specified value to the display name taken from it's <see cref="DisplayNameAttribute"/>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The display name of the value.</returns>
        [NotNull]
        public static string Convert([NotNull] object value)
        {
            Contract.Requires(value != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return InternalConvert(value, null, attr => attr.DisplayName);
        }

        /// <summary>
        /// Converts the specified value to the display name taken from it's <see cref="DisplayNameAttribute"/>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="enumType">An optional type of an enum to support converting <see cref="Enum"/> where the value is given as a number or string.</param>
        /// <returns>The display name of the value.</returns>
        [NotNull]
        public static string Convert([NotNull] object value, Type enumType)
        {
            Contract.Requires(value != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return InternalConvert(value, enumType, attr => attr.DisplayName);
        }
    }
}
