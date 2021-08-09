namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows.Data;

    using TomsToolbox.Essentials;

    /// <summary>
    /// A converter that converts the specified value by replacing text using a regular expression.
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class ReplaceTextConverter : ValueConverter
    {
        /// <summary>
        /// Gets or sets the regular expression to find.
        /// </summary>
        public string? Pattern { get; set; }

        /// <summary>
        /// Gets or sets the text to replace.
        /// </summary>
        public string? Replacement { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RegexOptions"/> used to find the string.
        /// </summary>
        public RegexOptions Options { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to replace all found instances or only the first.
        /// </summary>
        public bool ReplaceAll { get; set; }

        private object? Convert(string? value)
        {
            return Convert(value, Pattern, Replacement, Options, ReplaceAll);
        }

        /// <summary>
        /// Converts the specified value by replacing text using a regular expression.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="pattern">The regular expression to find.</param>
        /// <param name="replacement">The replacing text.</param>
        /// <param name="options">The options for the regular expression.</param>
        /// <param name="replaceAll">if set to <c>true</c> all occurrences will be replaces; otherwise only the first.</param>
        /// <returns>The converted value.</returns>
        public static object? Convert(string? value, string? pattern, string? replacement, RegexOptions options, bool replaceAll)
        {
            if (value == null)
                return null;

            if (pattern.IsNullOrEmpty())
                return value;

            replacement ??= string.Empty;

            var regex = new Regex(pattern, options);
            regex.Replace(value, replacement, replaceAll ? -1 : 1);

            return value;
        }

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
        protected override object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            return Convert(value as string);
        }
    }
}
