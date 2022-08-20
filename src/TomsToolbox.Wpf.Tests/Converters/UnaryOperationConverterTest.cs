namespace TomsToolbox.Wpf.Tests.Converters;

using System;
using System.Windows;

using Xunit;

using TomsToolbox.Wpf.Converters;

/// <summary>
/// Summary description for UnaryOperationConverterTest
/// </summary>
public class UnaryOperationConverterTest
{
    [Fact]
    public void UnaryOperation_Integer_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.Convert(1, null, null, null);

        Assert.Equal(-1, result);
    }

    [Fact]
    public void UnaryOperation_Double_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.Convert(42.5, null, null, null);

        Assert.Equal(-42.5, result);
    }

    [Fact]
    public void UnaryOperation_Timespan_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.Convert(TimeSpan.FromMinutes(3.14), null, null, null);

        Assert.Equal(TimeSpan.FromMinutes(-3.14), result);
    }

    [Fact]
    public void UnaryOperation_Vector_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.Convert(new Vector(3, 4), null, null, null);

        Assert.Equal(new Vector(-3, -4), result);
    }

    [Fact]
    public void UnaryOperation_StringWithTargetType_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.ConvertBack("-5", typeof(decimal), null, null);

        Assert.Equal(5m, result);
    }

}