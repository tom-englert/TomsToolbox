namespace TomsToolbox.Wpf.Tests.Converters
{
    using System.Windows.Media;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Wpf.Converters;

    [TestClass]
    public class StringToObjectConverterTests
    {
        [TestMethod]
        public void StringToObjectConverter_ConvertWithImplicitTargetType_Test()
        {
            var target = StringToObjectConverter.Default;

            var result = target.Convert("Blue", typeof(Color), null, null);

            Assert.AreEqual(Colors.Blue, result);
        }

        [TestMethod]
        public void StringToObjectConverter_ConvertWithExplicitTargetType_Test()
        {
            var target = new StringToObjectConverter { TypeConverterType = typeof(BrushConverter) };

            var result = target.Convert("Blue", null, null, null);

            Assert.AreEqual(Brushes.Blue, result);
        }

        [TestMethod]
        public void StringToObjectConverter_ConvertWithExplicitAndImplicitTargetType_Test()
        {
            var target = new StringToObjectConverter { TypeConverterType = typeof(BrushConverter) };

            var result = target.Convert("Blue", typeof(Color), null, null);

            Assert.AreEqual(Brushes.Blue, result);
        }
    }
}
