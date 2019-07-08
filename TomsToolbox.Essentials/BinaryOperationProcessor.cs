namespace TomsToolbox.Essentials
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

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
    /// If the value supports implicit or explicit casts, the operation is retried on all types that the type can be casted to. This enables the converter to handle most operations on Vector, Size, Point, etc...<para/>
    /// <para/>
    /// E.g. for System.Windows.Rect the <see cref="BinaryOperation.Addition"/> is mapped to "Rect.Offset(Vector)" and
    /// the <see cref="BinaryOperation.Multiply"/> is mapped to "Rect.Transform(Matrix)"
    /// </remarks>
    public sealed class BinaryOperationProcessor
    {
        // ReSharper disable AssignNullToNotNullAttribute : we will never call these with a null argument.
        [NotNull] private static readonly Func<object, object, object> _additionMethod = (a, b) => ToDouble(a) + ToDouble(b);
        [NotNull] private static readonly Func<object, object, object> _subtractionMethod = (a, b) => ToDouble(a) - ToDouble(b);
        [NotNull] private static readonly Func<object, object, object> _multiplyMethod = (a, b) => ToDouble(a) * ToDouble(b);
        [NotNull] private static readonly Func<object, object, object> _divisionMethod = (a, b) => ToDouble(a) / ToDouble(b);
        [NotNull] private static readonly Func<object, object, object> _equalityMethod = (a, b) => Equals(a, b);
        [NotNull] private static readonly Func<object, object, object> _inequalityMethod = (a, b) => !Equals(a, b);
        [NotNull] private static readonly Func<object, object, object> _greaterThanMethod = (a, b) => Compare(a, b) > 0;
        [NotNull] private static readonly Func<object, object, object> _lessThanMethod = (a, b) => Compare(a, b) < 0;
        [NotNull] private static readonly Func<object, object, object> _greaterThanOrEqualMethod = (a, b) => Compare(a, b) >= 0;
        [NotNull] private static readonly Func<object, object, object> _lessThanOrEqualMethod = (a, b) => Compare(a, b) <= 0;
        // ReSharper restore AssignNullToNotNullAttribute

        private readonly BinaryOperation _operation;
        [NotNull, ItemNotNull]
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
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

        }

        /// <summary>
        /// Gets the operation the converter is performing.
        /// </summary>
        public BinaryOperation Operation => _operation;

        /// <summary>
        /// Executes the operation.
        /// </summary>
        /// <param name="value1">The first value of the operation.</param>
        /// <param name="value2">The second value of the operation.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        [CanBeNull]
        public object Execute([CanBeNull] object value1, [CanBeNull] object value2)
        {
            if ((value1 == null) || (value2 == null))
                return value1;

            var valueType = value1.GetType();

            return ApplyOperation(valueType, value1, value2)
                ?? ApplyOperationOnCastedObject(valueType, value1, value2)
                ?? ApplyOperation(value1, value2);
        }

        [CanBeNull]
        private object ApplyOperation([CanBeNull] object value1, [CanBeNull] object value2)
        {
            return _operationMethod(value1, value2);
        }

        [CanBeNull]
        private object ApplyOperation([NotNull] Type valueType, [CanBeNull] object value1, [CanBeNull] object value2)
        {
            var methods = valueType.GetMethods(BindingFlags.Static | BindingFlags.Public);

            return methods
                .Where(m => _operationMethodNames.Contains(m?.Name))
                .Select(m => new { Method = m, Parameters = m?.GetParameters() })
                .Where(m => m.Parameters?.Length == 2)
                // ReSharper disable once PossibleNullReferenceException
                .Where(m => m.Parameters[0].ParameterType == valueType)
                // ReSharper disable once PossibleNullReferenceException
                .Select(m => ApplyOperation(m.Method, m.Parameters[1].ParameterType, value1, value2))
                .FirstOrDefault(v => v != null);
        }

        [CanBeNull]
        private object ApplyOperationOnCastedObject([NotNull] Type targetType, [CanBeNull] object value1, [CanBeNull] object value2)
        {
            // ReSharper disable PossibleNullReferenceException
            var result = targetType
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => (m.Name == "op_Explicit") || (m.Name == "op_Implicit"))
                .Select(m => new { Method = m, Parameters = m.GetParameters() })
                .Where(m => m.Parameters.Length == 1)
                .Where(m => m.Parameters[0].ParameterType == targetType)
                .Select(m => ApplyOperation(m.Method.ReturnType, m.Method.Invoke(null, new[] { value1 }), value2))
                .FirstOrDefault(v => v != null);
            // ReSharper restore PossibleNullReferenceException

            return result;
        }

        [CanBeNull]
        private static object ApplyOperation([NotNull] MethodInfo method, [NotNull] Type targetType, [CanBeNull] object value1, [CanBeNull] object value2)
        {
            try
            {
                if (value2?.GetType() == targetType)
                {
                    return method.Invoke(null, new[] { value1, value2 });
                }

                if (value2 is string parameterString)
                {
                    var typeConverter = TypeDescriptor.GetConverter(targetType);

                    value2 = typeConverter.ConvertFromInvariantString(parameterString);

                    return method.Invoke(null, new[] { value1, value2 });
                }

                value2 = Convert.ChangeType(value2, targetType, CultureInfo.InvariantCulture);

                return method.Invoke(null, new[] { value1, value2 });
            }
            catch
            {
                // Catch all and don't make assumptions about exceptions, we can't know what the method could throw.
            }

            return null;
        }

        private static double ToDouble([CanBeNull] object value)
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        [CanBeNull]
        private static object TryChangeType([CanBeNull] object value, [NotNull] Type targetType)
        {
            try
            {
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                // Catch all conversion errors..
            }

            return null;
        }

        private static int Compare([NotNull] object a, [CanBeNull] object b)
        {
            // ReSharper disable once PossibleNullReferenceException
            return Comparer.DefaultInvariant.Compare(a, Convert.ChangeType(b, a.GetType(), CultureInfo.InvariantCulture));
        }

        private new static bool Equals([NotNull] object a, [CanBeNull] object b)
        {
            object c;

            if ((c = TryChangeType(b, a.GetType())) != null)
                return a.Equals(c);

            return Math.Abs(ToDouble(a) - ToDouble(b)) < double.Epsilon;
        }
    }
}
