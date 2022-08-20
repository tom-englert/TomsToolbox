namespace TomsToolbox.Wpf.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Converts a double value into a star-size grid length.
/// </summary>
[ValueConversion(typeof(double), typeof(GridLength))]
public class StarSizeConverter : ValueConverter
{
    /// <inheritdoc />
    protected override object? Convert(object value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (value is not double size)
            return null;

        return new GridLength(size, GridUnitType.Star);
    }
}