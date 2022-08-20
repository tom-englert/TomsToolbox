namespace TomsToolbox.Wpf.Tests.Converters;

using System;
using System.Windows;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TomsToolbox.Wpf.Converters;

/// <summary>
/// Summary description for UnaryOperationConverterTest
/// </summary>
[TestClass]
public class UnaryOperationConverterTest
{
    [TestMethod]
    public void UnaryOperation_Integer_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.Convert(1, null, null, null);

        Assert.AreEqual(-1, result);
    }

    [TestMethod]
    public void UnaryOperation_Double_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.Convert(42.5, null, null, null);

        Assert.AreEqual(-42.5, result);
    }

    [TestMethod]
    public void UnaryOperation_Timespan_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.Convert(TimeSpan.FromMinutes(3.14), null, null, null);

        Assert.AreEqual(TimeSpan.FromMinutes(-3.14), result);
    }

    [TestMethod]
    public void UnaryOperation_Vector_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.Convert(new Vector(3, 4), null, null, null);

        Assert.AreEqual(new Vector(-3, -4), result);
    }

    [TestMethod]
    public void UnaryOperation_StringWithTargetType_Negation_Test()
    {
        var target = UnaryOperationConverter.Negation;

        var result = target.ConvertBack("-5", typeof(decimal), null, null);

        Assert.AreEqual(5m, result);
    }

}