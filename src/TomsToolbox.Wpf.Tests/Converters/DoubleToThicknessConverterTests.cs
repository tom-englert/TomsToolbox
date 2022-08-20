namespace TomsToolbox.Wpf.Tests.Converters;

using System.Windows;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TomsToolbox.Wpf.Converters;

[TestClass]
public class DoubleToThicknessConverterTests
{
    [TestMethod]
    public void DoubleToThicknessConverter_Convert_Test()
    {
        var target = DoubleToThicknessConverter.Default;
        var source = 2;
        var result = target.Convert(source, null, null, null);

        Assert.AreEqual(new Thickness(2,2,2,2), result);
    }

    [TestMethod]
    public void DoubleToThicknessConverter_ConvertWithParameter_Test()
    {
        var target = DoubleToThicknessConverter.Default;
        var source = 2;
        var parameter = "1,1,0,0";
        var result = target.Convert(source, null, parameter, null);

        Assert.AreEqual(new Thickness(2, 2, 0, 0), result);
    }

    [TestMethod]
    public void DoubleToThicknessConverter_ConvertWithInvalidParameter_Test()
    {
        var target = DoubleToThicknessConverter.Default;
        var source = 2;
        var parameter = new Rect();
        var result = target.Convert(source, null, parameter, null);

        Assert.AreEqual(DependencyProperty.UnsetValue, result);
    }

    [TestMethod]
    public void DoubleToThicknessConverter_ConvertWithInvalidSource_Test()
    {
        var target = DoubleToThicknessConverter.Default;
        var source = new Rect();
        var result = target.Convert(source, null, null, null);

        Assert.AreEqual(DependencyProperty.UnsetValue, result);
    }
}