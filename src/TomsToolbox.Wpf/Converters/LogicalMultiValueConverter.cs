namespace TomsToolbox.Wpf.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

/// <summary>
/// The logical operation performed by the <see cref="LogicalMultiValueConverter" />
/// </summary>
public enum LogicalOperation
{
    /// <summary>
    /// The logical AND operation; returns true if all items are true.
    /// </summary>
    And,
    /// <summary>
    /// The logical OR operation; returns true if any item is true.
    /// </summary>
    Or,
}

/// <summary>
/// A <see cref="IMultiValueConverter" /> that performs a logical operation on all items.
/// </summary>
/// <remarks>
/// All items must be convertible to boolean.
/// </remarks>
[ValueConversion(typeof(object[]), typeof(bool))]
public class LogicalMultiValueConverter : MultiValueConverter
{
    private static readonly Func<IEnumerable<bool>, bool> _andOperationMethod = items => items.All(item => item);
    private static readonly Func<IEnumerable<bool>, bool> _orOperationMethod = items => items.Any(item => item);

    private LogicalOperation _operation;
    private Func<IEnumerable<bool>, bool> _operationMethod = _andOperationMethod;

    /// <summary>
    /// The default logical AND converter. 
    /// </summary>
    public static readonly IMultiValueConverter And = new LogicalMultiValueConverter { Operation = LogicalOperation.And };
    /// <summary>
    /// The default logical OR converter. 
    /// </summary>
    public static readonly IMultiValueConverter Or = new LogicalMultiValueConverter { Operation = LogicalOperation.Or };

    /// <summary>
    /// Gets or sets the operation to be performed on all items.
    /// </summary>
    public LogicalOperation Operation
    {
        get => _operation;
        set
        {
            _operation = value;

            switch (value)
            {
                case LogicalOperation.And:
                    _operationMethod = _andOperationMethod;
                    break;

                case LogicalOperation.Or:
                    _operationMethod = _orOperationMethod;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    /// <summary>
    /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
    /// An input value of null will return null, whereas if the input array contains UnSet then UnSet will be returned.
    /// </summary>
    /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value.
    /// </returns>
    protected override object Convert(object?[] values, Type? targetType, object? parameter, CultureInfo? culture)
    {
        return _operationMethod(values.Select(v => System.Convert.ToBoolean(v, CultureInfo.InvariantCulture)));
    }
}