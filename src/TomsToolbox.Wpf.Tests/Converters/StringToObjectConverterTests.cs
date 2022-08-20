namespace TomsToolbox.Wpf.Tests.Converters;

using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using Xunit;

using TomsToolbox.Wpf.Converters;

public class StringToObjectConverterTests
{
    [Fact]
    public void StringToObjectConverter_ConvertWithImplicitTargetType_Test()
    {
        var target = StringToObjectConverter.Default;

        var result = target.Convert("Blue", typeof(Color), null, null);

        Assert.Equal(Colors.Blue, result);
    }

    [Fact]
    public void StringToObjectConverter_ConvertWithExplicitTargetType_Test()
    {
        IValueConverter target = new StringToObjectConverter { TypeConverterType = typeof(BrushConverter) };

        var result = target.Convert("Blue", null, null, null);

        Assert.Equal(Brushes.Blue, result);
    }

    [Fact]
    public void StringToObjectConverter_ConvertWithExplicitAndImplicitTargetType_Test()
    {
        IValueConverter target = new StringToObjectConverter { TypeConverterType = typeof(BrushConverter) };

        var result = target.Convert("Blue", typeof(Color), null, null);

        Assert.Equal(Brushes.Blue, result);
    }

    [Fact]
    public void StringToObjectConverter_ConvertWithBadInput_Test()
    {
        IValueConverter target = new StringToObjectConverter { TypeConverterType = typeof(BrushConverter) };

        var result = target.Convert("NoABrushName", null, null, null);

        Assert.Equal(DependencyProperty.UnsetValue, result);
    }
}