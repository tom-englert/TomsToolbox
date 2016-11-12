namespace TomsToolbox.Desktop
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;

    using JetBrains.Annotations;

    /// <summary>
    /// Applies the <see cref="Operation"/> on the values.<para/>
    /// </summary>
    /// <returns>
    /// If the conversions succeed, the result of the operation is returned. If any error occurs, the result is null.
    /// </returns>
    /// <remarks>
    /// This processor works with different types on both sides.<para/>
    /// Either<para/>
    /// - both values must be convertible to a double<para/>
    /// or<para/> 
    /// - value1 must have an explicit operator for the specified operation and value2 has a type converter matching the expected operator parameter.<para/>
    /// If the value supports implicit or explicit casts, the operation is retried on all types that the type can be casted to. This enables the converter to handle most operations on <see cref="Vector"/>, <see cref="Size"/>, <see cref="Point"/>, etc...<para/>
    /// <para/> 
    /// For <see cref="Rect"/> the <see cref="BinaryOperation.Addition"/> is mapped to <see cref="Rect.Offset(Vector)"/> and
    /// the <see cref="BinaryOperation.Multiply"/> is mapped to <see cref="Rect.Transform(Matrix)"/>
    /// </remarks>
    public sealed class BinaryOperationProcessor
    {
        private static readonly Func<object, object, object> _additionMethod = (a, b) => ToDouble(a) + ToDouble(b);
        private static readonly Func<object, object, object> _subtractionMethod = (a, b) => ToDouble(a) - ToDouble(b);
        private static readonly Func<object, object, object> _multiplyMethod = (a, b) => ToDouble(a) * ToDouble(b);
        private static readonly Func<object, object, object> _divisionMethod = (a, b) => ToDouble(a) / ToDouble(b);
        private static readonly Func<object, object, object> _equalityMethod = (a, b) => Equals(a, b);
        private static readonly Func<object, object, object> _inequalityMethod = (a, b) => !Equals(a, b);
        private static readonly Func<object, object, object> _greaterThanMethod = (a, b) => Compare(a, b) > 0;
        private static readonly Func<object, object, object> _lessThanMethod = (a, b) => Compare(a, b) < 0;
        private static readonly Func<object, object, object> _greaterThanOrEqualMethod = (a, b) => Compare(a, b) >= 0;
        private static readonly Func<object, object, object> _lessThanOrEqualMethod = (a, b) => Compare(a, b) <= 0;

        private readonly BinaryOperation _operation;
        [NotNull]
        private readonly string[] _operationMethodNames;
        [NotNull]
        private readonly Func<object, object, object> _operationMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperationProcessor"/> class.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">operation</exception>
        public BinaryOperationProcessor(BinaryOperation operation)
        {
            _operation = operation;
            _operationMethodNames = new[] { "op_" + operation };

            switch (operation)
            {
                case BinaryOperation.Addition:
                    _operationMethod = _additionMethod;
                    _operationMethodNames = new[] { "op_" + operation, "Offset" };
                    break;

                case BinaryOperation.Subtraction:
                    _operationMethod = _subtractionMethod;
                    break;

                case BinaryOperation.Multiply:
                    _operationMethod = _multiplyMethod;
                    _operationMethodNames = new[] { "op_" + operation, "Transform" };
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

                case BinaryOperation.GreaterThanOrEqual:
                    _operationMethod = _greaterThanOrEqualMethod;
                    break;

                case BinaryOperation.LessThanOrEqual:
                    _operationMethod = _lessThanOrEqualMethod;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("operation", operation, null);
            }

        }

        /// <summary>
        /// Gets the operation the converter is performing.
        /// </summary>
        public BinaryOperation Operation
        {
            get
            {
                return _operation;
            }
        }

        /// <summary>
        /// Executes the operation.
        /// </summary>
        /// <param name="value1">The first value of the operation.</param>
        /// <param name="value2">The second value of the operation.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public object Execute(object value1, object value2)
        {
            if ((value1 == null) || (value2 == null))
                return value1;

            var valueType = value1.GetType();

            return ApplyOperation(valueType, value1, value2)
                ?? ApplyOperationOnCastedObject(valueType, value1, value2)
                ?? ApplyOperation(value1, value2);
        }

        private object ApplyOperation([NotNull] object value1, [NotNull] object value2)
        {
            Contract.Requires(value1 != null);
            Contract.Requires(value2 != null);

            return _operationMethod(value1, value2);
        }

        private object ApplyOperation([NotNull] Type valueType, [NotNull] object value1, [NotNull] object value2)
        {
            Contract.Requires(valueType != null);
            Contract.Requires(value1 != null);
            Contract.Requires(value2 != null);

            var methods = valueType.GetMethods(BindingFlags.Static | BindingFlags.Public);

            return methods
                .Where(m => _operationMethodNames.Contains(m.Name))
                .Select(m => new { Method = m, Parameters = m.GetParameters() })
                .Where(m => m.Parameters.Length == 2)
                .Where(m => m.Parameters[0].ParameterType == valueType)
                .Select(m => ApplyOperation(m.Method, m.Parameters[1].ParameterType, value1, value2))
                .FirstOrDefault(v => v != null);
        }

        private object ApplyOperationOnCastedObject([NotNull] Type targetType, [NotNull] object value1, [NotNull] object value2)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(value1 != null);
            Contract.Requires(value2 != null);

            var result = targetType
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => (m.Name == "op_Explicit") || (m.Name == "op_Implicit"))
                .Select(m => new { Method = m, Parameters = m.GetParameters() })
                .Where(m => m.Parameters.Length == 1)
                .Where(m => m.Parameters[0].ParameterType == targetType)
                .Select(m => ApplyOperation(m.Method.ReturnType, m.Method.Invoke(null, new[] { value1 }), value2))
                .FirstOrDefault(v => v != null);

            return result;
        }

        private static object ApplyOperation([NotNull] MethodInfo method, [NotNull] Type targetType, [NotNull] object value1, [NotNull] object value2)
        {
            Contract.Requires(method != null);
            Contract.Requires(value1 != null);
            Contract.Requires(targetType != null);
            Contract.Requires(value2 != null);

            try
            {
                if (value2.GetType() == targetType)
                {
                    return method.Invoke(null, new[] { value1, value2 });
                }

                var parameterString = value2 as string;
                if (parameterString != null)
                {
                    var typeConverter = TypeDescriptor.GetConverter(targetType);

                    value2 = typeConverter.ConvertFromInvariantString(parameterString);

                    return method.Invoke(null, new[] { value1, value2 });
                }

                value2 = System.Convert.ChangeType(value2, targetType, CultureInfo.InvariantCulture);
                return method.Invoke(null, new[] { value1, value2 });
            }
            catch
            {
            }

            return null;
        }

        private static double ToDouble(object value)
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        private static object TryChangeType(object value, [NotNull] Type targetType)
        {
            Contract.Requires(targetType != null);

            try
            {
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            return null;
        }

        private static int Compare([NotNull] object a, object b)
        {
            Contract.Requires(a != null);
            return Comparer.DefaultInvariant.Compare(a, Convert.ChangeType(b, a.GetType(), CultureInfo.InvariantCulture));
        }

        private new static bool Equals([NotNull] object a, object b)
        {
            Contract.Requires(a != null);

            object c;

            if ((c = TryChangeType(b, a.GetType())) != null)
                return a.Equals(c);

            return Math.Abs(ToDouble(a) - ToDouble(b)) < double.Epsilon;
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_operationMethodNames != null);
            Contract.Invariant(_operationMethod != null);
        }
    }
}
