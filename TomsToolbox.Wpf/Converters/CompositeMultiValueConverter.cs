﻿namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// A <see cref="IMultiValueConverter"/> that chains one <see cref="IMultiValueConverter"/> with a list of <see cref="IValueConverter"/>.
    /// The <see cref="CompositeMultiValueConverter.MultiValueConverter"/> is invoked first, and the result is converted by the <see cref="CompositeMultiValueConverter.Converters"/> in the specified order.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Use the same term as in IMultiValueConverter")]
    [ContentProperty("Converters")]
    [ValueConversion(typeof(object[]), typeof(object))]
    public class CompositeMultiValueConverter : IMultiValueConverter
    {
        private readonly CompositeConverter _compositeConverter = new CompositeConverter();

        /// <summary>
        /// Gets or sets the multi value converter.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Use the same term as in IMultiValueConverter")]
        public IMultiValueConverter MultiValueConverter
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets the list of converters.
        /// </summary>
        public Collection<IValueConverter> Converters
        {
            get
            {
                Contract.Ensures(Contract.Result<Collection<IValueConverter>>() != null);

                return _compositeConverter.Converters;
            }
        }

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (MultiValueConverter == null)
                throw new InvalidOperationException("A MultiValueConverter must be set.");

            return _compositeConverter.Convert(MultiValueConverter.Convert(values, targetType, parameter, culture), targetType, parameter, culture);
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
