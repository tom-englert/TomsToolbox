namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// The arithmetic operation performed by the <see cref="ArithmeticMultiValueConverter" />
    /// </summary>
    public enum ArithmeticOperation
    {
        /// <summary>
        /// The arithmetic MIN operation; returns the minimum of all items.
        /// </summary>
        Min,
        /// <summary>
        /// The arithmetic MAX operation; returns the maximum of all items.
        /// </summary>
        Max,
        /// <summary>
        /// The arithmetic sum operation; returns the sum of all items.
        /// </summary>
        Sum,
        /// <summary>
        /// The arithmetic average operation; returns the average of all items.
        /// </summary>
        Average,
    }

    /// <summary>
    /// A <see cref="IMultiValueConverter" /> that performs a arithmetic operation on all items.
    /// </summary>
    /// <remarks>
    /// All items must be convertible to double.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Use the same term as in IMultiValueConverter")]
    public class ArithmeticMultiValueConverter : IMultiValueConverter
    {
        private static readonly Func<IEnumerable<double>, double> _minOperationMethod = items => items.DefaultIfEmpty().Min();
        private static readonly Func<IEnumerable<double>, double> _maxOperationMethod = items => items.DefaultIfEmpty().Max();
        private static readonly Func<IEnumerable<double>, double> _sumOperationMethod = items => items.DefaultIfEmpty().Sum();
        private static readonly Func<IEnumerable<double>, double> _averageOperationMethod = items => items.DefaultIfEmpty().Average();

        private ArithmeticOperation _operation;
        private Func<IEnumerable<double>, double> _operationMethod = _minOperationMethod;

        /// <summary>
        /// The default arithmetic MIN converter. 
        /// </summary>
        public static readonly IMultiValueConverter Min = new ArithmeticMultiValueConverter { Operation = ArithmeticOperation.Min };
        /// <summary>
        /// The default arithmetic MAX converter. 
        /// </summary>
        public static readonly IMultiValueConverter Max = new ArithmeticMultiValueConverter { Operation = ArithmeticOperation.Max };
        /// <summary>
        /// The default arithmetic SUM converter. 
        /// </summary>
        public static readonly IMultiValueConverter Sum = new ArithmeticMultiValueConverter { Operation = ArithmeticOperation.Sum };
        /// <summary>
        /// The default arithmetic AVERAGE converter. 
        /// </summary>
        public static readonly IMultiValueConverter Average = new ArithmeticMultiValueConverter { Operation = ArithmeticOperation.Average };

        /// <summary>
        /// Gets or sets the operation to be performed on all items.
        /// </summary>
        public ArithmeticOperation Operation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;

                switch (value)
                {
                    case ArithmeticOperation.Min:
                        _operationMethod = _minOperationMethod;
                        break;

                    case ArithmeticOperation.Max:
                        _operationMethod = _maxOperationMethod;
                        break;

                    case ArithmeticOperation.Sum:
                        _operationMethod = _sumOperationMethod;
                        break;

                    case ArithmeticOperation.Average:
                        _operationMethod = _averageOperationMethod;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("value", value, null);
                }
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
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;

            try
            {
                return _operationMethod(values.Select(v => System.Convert.ToDouble(v, CultureInfo.InvariantCulture)));
            }
            catch (SystemException)
            {
            }

            return null;
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
        /// <exception cref="System.NotImplementedException"></exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_operationMethod != null);
        }
    }
}