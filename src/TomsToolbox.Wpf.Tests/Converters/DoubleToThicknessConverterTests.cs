namespace TomsToolbox.Wpf.Tests.Converters;

using System.Windows;

using Xunit;

using TomsToolbox.Wpf.Converters;

public class DoubleToThicknessConverterTests
{
    [Fact]
    public void DoubleToThicknessConverter_Convert_Test()
    {
        var target = DoubleToThicknessConverter.Default;
        var source = 2;
        var result = target.Convert(source, null, null, null);

        Assert.Equal(new Thickness(2,2,2,2), result);
    }

    [Fact]
    public void DoubleToThicknessConverter_ConvertWithParameter_Test()
    {
        var target = DoubleToThicknessConverter.Default;
        var source = 2;
        var parameter = "1,1,0,0";
        var result = target.Convert(source, null, parameter, null);

        Assert.Equal(new Thickness(2, 2, 0, 0), result);
    }

    [Fact]
    public void DoubleToThicknessConverter_ConvertWithInvalidParameter_Test()
    {
        var target = DoubleToThicknessConverter.Default;
        var source = 2;
        var parameter = new Rect();
        var result = target.Convert(source, null, parameter, null);

        Assert.Equal(DependencyProperty.UnsetValue, result);
    }

    [Fact]
    public void DoubleToThicknessConverter_ConvertWithInvalidSource_Test()
    {
        var target = DoubleToThicknessConverter.Default;
        var source = new Rect();
        var result = target.Convert(source, null, null, null);

        Assert.Equal(DependencyProperty.UnsetValue, result);
    }
}