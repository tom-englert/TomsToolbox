namespace TomsToolbox.Wpf.Tests.Converters;

using System;
using System.Windows.Data;

using Xunit;

using TomsToolbox.Wpf.Converters;

public class AggregatingMultiValueConverterTests
{
    [Fact]
    public void AggregatingMultiValueConverter_CalculateTimeThreshold_ConditionOK()
    {
        IMultiValueConverter target = new AggregatingMultiValueConverter
        {
            Converters =
            {
                BinaryOperationConverter.Subtraction,
                BinaryOperationConverter.LessThan
            }
        };

        var now = DateTime.Now;
        var eventTime = now - TimeSpan.FromDays(2.3);
        var threshold = TimeSpan.FromDays(2.4);

        var result = target.Convert(new object[] { now, eventTime, threshold }, null, null, null);

        Assert.Equal(true, result);
    }

    [Fact]
    public void AggregatingMultiValueConverter_CalculateTimeThreshold_ConditionFails()
    {
        IMultiValueConverter target = new AggregatingMultiValueConverter
        {
            Converters =
            {
                BinaryOperationConverter.Subtraction,
                BinaryOperationConverter.LessThan
            }
        };

        var now = DateTime.Now;
        var eventTime = now - TimeSpan.FromDays(2.3);
        var threshold = TimeSpan.FromDays(2.2);

        var result = target.Convert(new object[] { now, eventTime, threshold }, null, null, null);

        Assert.Equal(false, result);
    }

    [Fact]
    public void AggregatingMultiValueConverter_CascadingMultiConverters()
    {
        IMultiValueConverter target = new AggregatingMultiValueConverter
        {
            Converters =
            {
                BinaryOperationConverter.Subtraction,
                BinaryOperationConverter.Division,
                ArithmeticMultiValueConverter.Product
            }
        };

        var input = new object[] { 9, 3, 2, 3, 2 };
        var result = target.Convert(input, null, null, null);

        // ((9 - 3) / 2) * 3 * 2
        Assert.Equal(18.0, result);
    }

    [Fact]
    public void AggregatingMultiValueConverter_RepeatingLastConverter()
    {
        IMultiValueConverter target = new AggregatingMultiValueConverter
        {
            Converters =
            {
                BinaryOperationConverter.Subtraction,
                BinaryOperationConverter.Division,
            }
        };

        var input = new object[] { 9, 3, 2, 3, 2 };
        var result = target.Convert(input, null, null, null);

        // (((9 - 3) / 2) / 3) / 2
        Assert.Equal(.5, result);
    }

}
