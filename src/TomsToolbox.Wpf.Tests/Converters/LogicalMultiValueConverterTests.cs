namespace TomsToolbox.Wpf.Tests.Converters;

using System.Windows;

using Xunit;

using TomsToolbox.Wpf.Converters;

public class LogicalMultiValueConverterTests
{
    [Fact]
    public void LogicalMultiValueConverter_And_1_Test()
    {
        var target = LogicalMultiValueConverter.And;
        var source = new object[] { true, true, false };

        var result = target.Convert(source, null, null, null);

        Assert.Equal(false, result);
    }

    [Fact]
    public void LogicalMultiValueConverter_And_2_Test()
    {
        var target = LogicalMultiValueConverter.And;
        var source = new object[] { true, true, true };

        var result = target.Convert(source, null, null, null);

        Assert.Equal(true, result);
    }

    [Fact]
    public void LogicalMultiValueConverter_And_MixedInput1_Test()
    {
        var target = LogicalMultiValueConverter.And;
        var source = new object[] { true, 1, "true", 2 };
        var result = target.Convert(source, null, null, null);

        Assert.Equal(true, result);
    }

    [Fact]
    public void LogicalMultiValueConverter_And_MixedInput2_Test()
    {
        var target = LogicalMultiValueConverter.And;
        var source = new object[] { true, 1, "true", 0 };
        var result = target.Convert(source, null, null, null);

        Assert.Equal(false, result);
    }

    [Fact]
    public void LogicalMultiValueConverter_Or_1_Test()
    {
        var target = LogicalMultiValueConverter.Or;
        var source = new object[] { true, true, false };

        var result = target.Convert(source, null, null, null);

        Assert.Equal(true, result);
    }

    [Fact]
    public void LogicalMultiValueConverter_Or_2_Test()
    {
        var target = LogicalMultiValueConverter.Or;
        var source = new object[] { true, true, true };

        var result = target.Convert(source, null, null, null);

        Assert.Equal(true, result);
    }

    [Fact]
    public void LogicalMultiValueConverter_Or_MixedInput1_Test()
    {
        var target = LogicalMultiValueConverter.Or;
        var source = new object[] { true, 1, "true", 0, "false" };
        var result = target.Convert(source, null, null, null);

        Assert.Equal(true, result);
    }

    [Fact]
    public void LogicalMultiValueConverter_Or_MixedInput2_Test()
    {
        var target = LogicalMultiValueConverter.Or;
        var source = new object[] { true, 1, "true", 0 };
        var result = target.Convert(source, null, null, null);

        Assert.Equal(true, result);
    }

    [Fact]
    public void LogicalMultiValueConverter_And_InvalidInput_Test()
    {
        var target = LogicalMultiValueConverter.And;
        var source = new object[] { true, 1, "Test" };

        var result = target.Convert(source, null, null, null);

        Assert.Equal(DependencyProperty.UnsetValue, result);
    }

    [Fact]
    public void LogicalMultiValueConverter_Or_InvalidInput_Test()
    {
        var target = LogicalMultiValueConverter.Or;
        var source = new object[] { true, 1, "Test" };

        var result = target.Convert(source, null, null, null);

        Assert.Equal(true, result);
    }
}