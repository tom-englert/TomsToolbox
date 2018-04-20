namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    using JetBrains.Annotations;

    using TomsToolbox.Desktop;

    /// <summary>
    /// Applies the <see cref="BinaryOperationConverter.Operation"/> on the value and the converter parameter.<para/>
    /// May also be used as <see cref="IMultiValueConverter"/> where both operands are specified using bindings.
    /// </summary>
    /// <returns>
    /// If the conversions succeed, the result of the operation is returned. If any error occurs, the result is <see cref="DependencyProperty.UnsetValue"/>.
    /// </returns>
    /// <remarks>
    /// Either<para/>
    /// - both value and parameter must be convertible to a double<para/>
    /// or<para/> 
    /// - value must have an explicit operator for the specified operation and parameter has a type converter matching the expected operator parameter.<para/>
    /// If the value supports implicit or explicit casts, the operation is retried on all types that the original type can be casted to. This enables the converter to handle most operations on <see cref="Vector"/>, <see cref="Size"/>, <see cref="Point"/>, etc...<para/>
    /// <para/> 
    /// For <see cref="Rect"/> the <see cref="BinaryOperation.Addition"/> is mapped to <see cref="Rect.Offset(Vector)"/> and
    /// the <see cref="BinaryOperation.Multiply"/> is mapped to <see cref="Rect.Transform(Matrix)"/>
    /// </remarks>
    [ValueConversion(typeof(object), typeof(object))]
    public class BinaryOperationConverter : ValueConverter, IMultiValueConverter
    {
        [CanBeNull]
        private BinaryOperationProcessor _processor;

        /// <summary>
        /// The default addition converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter Addition = new BinaryOperationConverter { Operation = BinaryOperation.Addition };
        /// <summary>
        /// The default subtraction converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter Subtraction = new BinaryOperationConverter { Operation = BinaryOperation.Subtraction };
        /// <summary>
        /// The default multiplication converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter Multiply = new BinaryOperationConverter { Operation = BinaryOperation.Multiply };
        /// <summary>
        /// The default division converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter Division = new BinaryOperationConverter { Operation = BinaryOperation.Division };
        /// <summary>
        /// The default equality converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter Equality = new BinaryOperationConverter { Operation = BinaryOperation.Equality };
        /// <summary>
        /// The default inequality converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter Inequality = new BinaryOperationConverter { Operation = BinaryOperation.Inequality };
        /// <summary>
        /// The default greater than converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter GreaterThan = new BinaryOperationConverter { Operation = BinaryOperation.GreaterThan };
        /// <summary>
        /// The default less than converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter LessThan = new BinaryOperationConverter { Operation = BinaryOperation.LessThan };
        /// <summary>
        /// The default greater than or equals converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter GreaterThanOrEqual = new BinaryOperationConverter { Operation = BinaryOperation.GreaterThanOrEqual };
        /// <summary>
        /// The default less than or equals converter.
        /// </summary>
        [NotNull] public static readonly IValueConverter LessThanOrEqual = new BinaryOperationConverter { Operation = BinaryOperation.LessThanOrEqual };

        /// <summary>
        /// Gets or sets the operation the converter is performing.
        /// </summary>
        public BinaryOperation Operation
        {
            get => Processor.Operation;
            set => _processor = new BinaryOperationProcessor(value);
        }

        [NotNull]
        private BinaryOperationProcessor Processor
        {
            get
            {
                return _processor ?? (_processor = new BinaryOperationProcessor(BinaryOperation.Addition));
            }
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
            if (parameter == null)
                return value;

            return Processor.Execute(value, parameter);
        }

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.
        /// </returns>
        /// <exception cref="System.ArgumentException">MultiValueConverter requires two values.;values</exception>
        [CanBeNull]
        public object Convert([CanBeNull] object[] values, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
        {
            if (values == null)
                return null;

            if (values.Any(value => (value == null) || (value == DependencyProperty.UnsetValue)))
                return DependencyProperty.UnsetValue;

            if (values.Length != 2)
                throw new ArgumentException("MultiValueConverter requires two values.", nameof(values));

            return Processor.Execute(values[0], values[1]);
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
        /// <exception cref="System.InvalidOperationException">This operation is not supported.</exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
