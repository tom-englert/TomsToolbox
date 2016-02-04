using System.Collections.Generic;

namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Binary operations supported by the <see cref="BinaryOperationConverter"/>
    /// </summary>
    public enum BinaryOperation
    {
        /// <summary>
        /// The addition operation.
        /// </summary>
        Addition,
        /// <summary>
        /// The subtraction operation.
        /// </summary>
        Subtraction,
        /// <summary>
        /// The multiply operation.
        /// </summary>
        Multiply,
        /// <summary>
        /// The division operation.
        /// </summary>
        Division,
        /// <summary>
        /// The equality operation.
        /// </summary>
        Equality,
        /// <summary>
        /// The inequality operation.
        /// </summary>
        Inequality,
        /// <summary>
        /// The greater than operation.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// The less than operation.
        /// </summary>
        LessThan,
        /// <summary>
        /// The greater than or equals operation.
        /// </summary>
        GreaterThanOrEquals,
        /// <summary>
        /// The less than or equals operation.
        /// </summary>
        LessThanOrEquals
    }

    /// <summary>
    /// Applies the <see cref="BinaryOperationConverter.Operation"/> on the value and the converter parameter.
    /// </summary>
    /// <returns>
    /// If the conversions succeed, the result of the operation is returned. If any error occurs, the result is null.
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
    public class BinaryOperationConverter : IValueConverter
    {
        private static readonly Func<object, object, object> _additionMethod = (a, b) => ToDouble(a) + ToDouble(b);
        private static readonly Func<object, object, object> _subtractionMethod = (a, b) => ToDouble(a) - ToDouble(b);
        private static readonly Func<object, object, object> _multiplyMethod = (a, b) => ToDouble(a) * ToDouble(b);
        private static readonly Func<object, object, object> _divisionMethod = (a, b) => ToDouble(a) / ToDouble(b);
        private static readonly Func<object, object, object> _equalityMethod = (a, b) => a.GetType() == b.GetType() ? a.Equals(b) : Math.Abs(ToDouble(a) - ToDouble(b)) < Double.Epsilon;
        private static readonly Func<object, object, object> _inequalityMethod = (a, b) => a.GetType() == b.GetType() ? !a.Equals(b) : Math.Abs(ToDouble(a) - ToDouble(b)) > Double.Epsilon;
        private static readonly Func<object, object, object> _greaterThanMethod = (a, b) => ToDouble(a) > ToDouble(b);
        private static readonly Func<object, object, object> _lessThanMethod = (a, b) => ToDouble(a) < ToDouble(b);
        private static readonly Func<object, object, object> _greaterThanOrEqualsMethod = (a, b) => ToDouble(a) >= ToDouble(b);
        private static readonly Func<object, object, object> _lessThanOrEqualsMethod = (a, b) => ToDouble(a) <= ToDouble(b);

        private static Dictionary<Func<object, object, object>, Func<object, object, object>> _inverseOperations =
            new Dictionary<Func<object, object, object>, Func<object, object, object>>();

        static BinaryOperationConverter() {
            _inverseOperations.Add(_additionMethod, _subtractionMethod);
            _inverseOperations.Add(_subtractionMethod, _additionMethod);
            _inverseOperations.Add(_multiplyMethod, _divisionMethod);
            _inverseOperations.Add(_divisionMethod, _multiplyMethod);
            _inverseOperations.Add(_equalityMethod, _inequalityMethod);
            _inverseOperations.Add(_inequalityMethod, _equalityMethod);
            _inverseOperations.Add(_greaterThanMethod, _lessThanOrEqualsMethod);
            _inverseOperations.Add(_lessThanOrEqualsMethod, _greaterThanMethod);
            _inverseOperations.Add(_lessThanMethod, _greaterThanOrEqualsMethod);
            _inverseOperations.Add(_greaterThanOrEqualsMethod, _lessThanMethod);
        }

        private BinaryOperation _operation;
        private string[] _operationMethodNames = { "op_Addition", "Offset" };
        private Func<object, object, object> _operationMethod = _additionMethod;

        private Func<object, object, object> GetOperationMethod(bool inverse)
        {
            if (!inverse)
                return _operationMethod;
            return _inverseOperations[_operationMethod];
        }

        /// <summary>
        /// The default addition converter.
        /// </summary>
        public static readonly IValueConverter Addition = new BinaryOperationConverter { Operation = BinaryOperation.Addition };
        /// <summary>
        /// The default subtraction converter.
        /// </summary>
        public static readonly IValueConverter Subtraction = new BinaryOperationConverter { Operation = BinaryOperation.Subtraction };
        /// <summary>
        /// The default multiplication converter.
        /// </summary>
        public static readonly IValueConverter Multiply = new BinaryOperationConverter { Operation = BinaryOperation.Multiply };
        /// <summary>
        /// The default division converter.
        /// </summary>
        public static readonly IValueConverter Division = new BinaryOperationConverter { Operation = BinaryOperation.Division };
        /// <summary>
        /// The default equality converter.
        /// </summary>
        public static readonly IValueConverter Equality = new BinaryOperationConverter { Operation = BinaryOperation.Equality };
        /// <summary>
        /// The default inequality converter.
        /// </summary>
        public static readonly IValueConverter Inequality = new BinaryOperationConverter { Operation = BinaryOperation.Inequality };
        /// <summary>
        /// The default greater than converter.
        /// </summary>
        public static readonly IValueConverter GreaterThan = new BinaryOperationConverter { Operation = BinaryOperation.GreaterThan };
        /// <summary>
        /// The default less than converter.
        /// </summary>
        public static readonly IValueConverter LessThan = new BinaryOperationConverter { Operation = BinaryOperation.LessThan };
        /// <summary>
        /// The default greater than or equals converter.
        /// </summary>
        public static readonly IValueConverter GreaterThanOrEquals = new BinaryOperationConverter { Operation = BinaryOperation.GreaterThanOrEquals };
        /// <summary>
        /// The default less than or equals converter.
        /// </summary>
        public static readonly IValueConverter LessThanOrEquals = new BinaryOperationConverter { Operation = BinaryOperation.LessThanOrEquals };

        /// <summary>
        /// Gets or sets the operation the converter is performing.
        /// </summary>
        public BinaryOperation Operation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;
                _operationMethodNames = new[] { "op_" + value };
                switch (value)
                {
                    case BinaryOperation.Addition:
                        _operationMethod = _additionMethod;
                        _operationMethodNames = new[] { "op_" + value, "Offset" };
                        break;

                    case BinaryOperation.Subtraction:
                        _operationMethod = _subtractionMethod;
                        break;

                    case BinaryOperation.Multiply:
                        _operationMethod = _multiplyMethod;
                        _operationMethodNames = new[] { "op_" + value, "Transform" };
                        break;

                    case BinaryOperation.Division:
                        _operationMethod = _divisionMethod;
                        break;

                    case BinaryOperation.Equality:
                        _operationMethod = _equalityMethod;
                        break;

                    case BinaryOperation.Inequality:
                        _operationMethod = _inequalityMethod;
                        break;

                    case BinaryOperation.GreaterThan:
                        _operationMethod = _greaterThanMethod;
                        break;

                    case BinaryOperation.LessThan:
                        _operationMethod = _lessThanMethod;
                        break;

                    case BinaryOperation.GreaterThanOrEquals:
                        _operationMethod = _greaterThanOrEqualsMethod;
                        break;

                    case BinaryOperation.LessThanOrEquals:
                        _operationMethod = _lessThanOrEqualsMethod;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        private object InternalConvert(object value, Type targetType, object parameter, CultureInfo culture,
            bool inverse)
        {
            if (value == DependencyProperty.UnsetValue)
                return value;

            if ((value == null) || (parameter == null))
                return value;

            var valueType = value.GetType();

            // TODO: I'm not sure where inverse should be plugged in for ApplyOperation and ApplyOperationOnCastedObject
            if (Type.GetTypeCode(valueType) == TypeCode.Object) {
                return ApplyOperation(valueType, value, parameter) ?? ApplyOperationOnCastedObject(valueType, value, parameter) ?? ApplyOperation(value, parameter, inverse);
            }

            return ApplyOperation(value, parameter, inverse);
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return InternalConvert(value, targetType, parameter, culture, false);
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
            // TODO: I'm not sure where inverse should be plugged in for ApplyOperation and ApplyOperationOnCastedObject
            // once that is fixed enabled this line
            //return InternalConvert(value, targetType, parameter, culture, true);
            throw new InvalidOperationException();
        }

        private object ApplyOperation(object value, object parameter, bool inverse)
        {
            Contract.Requires(value != null);
            Contract.Requires(parameter != null);

            try
            {
                return GetOperationMethod(inverse)(value, parameter);
            }
            catch (SystemException)
            {
            }

            return null;
        }

        private object ApplyOperation(Type valueType, object value, object parameter)
        {
            Contract.Requires(valueType != null);
            Contract.Requires(value != null);
            Contract.Requires(parameter != null);

            var methods = valueType.GetMethods(BindingFlags.Static | BindingFlags.Public);

            return methods
                .Where(m => _operationMethodNames.Contains(m.Name))
                .Select(m => new { Method = m, Parameters = m.GetParameters() })
                .Where(m => m.Parameters.Length == 2)
                .Where(m => m.Parameters[0].ParameterType == valueType)
                .Select(m => ApplyOperation(m.Method, value, m.Parameters[1].ParameterType, parameter))
                .FirstOrDefault(v => v != null);
        }

        private object ApplyOperationOnCastedObject(Type valueType, object value, object parameter)
        {
            Contract.Requires(valueType != null);
            Contract.Requires(value != null);
            Contract.Requires(parameter != null);

            var result = valueType
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => (m.Name == "op_Explicit") || (m.Name == "op_Implicit"))
                .Select(m => new { Method = m, Parameters = m.GetParameters() })
                .Where(m => m.Parameters.Length == 1)
                .Where(m => m.Parameters[0].ParameterType == valueType)
                .Select(m => ApplyOperation(m.Method.ReturnType, m.Method.Invoke(null, new[] { value }), parameter))
                .FirstOrDefault(v => v != null);

            return result;
        }

        private static object ApplyOperation(MethodInfo method, object value, Type parameterType, object parameter)
        {
            Contract.Requires(method != null);
            Contract.Requires(value != null);
            Contract.Requires(parameterType != null);
            Contract.Requires(parameter != null);

            try
            {
                if (parameter.GetType() == parameterType)
                {
                    return method.Invoke(null, new[] { value, parameter });
                }

                var parameterString = parameter as string;
                if (parameterString != null)
                {
                    var typeConverter = TypeDescriptor.GetConverter(parameterType);

                    parameter = typeConverter.ConvertFromInvariantString(parameterString);

                    return method.Invoke(null, new[] { value, parameter });
                }

                parameter = System.Convert.ChangeType(parameter, parameterType, CultureInfo.InvariantCulture);
                return method.Invoke(null, new[] { value, parameter });
            }
            catch
            {
            }

            return null;
        }

        private static double ToDouble(object value)
        {
            return System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_operationMethodNames != null);
            Contract.Invariant(_operationMethod != null);
        }
    }
}
