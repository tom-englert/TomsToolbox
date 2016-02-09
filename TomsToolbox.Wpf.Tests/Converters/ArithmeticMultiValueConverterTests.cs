namespace TomsToolbox.Wpf.Tests.Converters
{
    using System.Windows;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Wpf.Converters;

    [TestClass]
    public class ArithmeticMultiValueConverterTests
    {
        [TestMethod]
        public void ArithmeticMultiValueConverter_Min_Test()
        {
            var target = ArithmeticMultiValueConverter.Min;
            var source = new object[] { 10, 12, "3.5" };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(3.5, result);
        }

        [TestMethod]
        public void ArithmeticMultiValueConverter_Min_InvalidInput_Test()
        {
            var target = ArithmeticMultiValueConverter.Min;
            var source = new object[] { 10, 12, "3.5", "NAN" };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(DependencyProperty.UnsetValue, result);
        }

        [TestMethod]
        public void ArithmeticMultiValueConverter_Max_Test()
        {
            var target = ArithmeticMultiValueConverter.Max;
            var source = new object[] { 10, 12, "3.5" };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(12.0, result);
        }

        [TestMethod]
        public void ArithmeticMultiValueConverter_Max_InvalidInput_Test()
        {
            var target = ArithmeticMultiValueConverter.Max;
            var source = new object[] { 10, 12, "3.5", "NAN" };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(DependencyProperty.UnsetValue, result);
        }

        [TestMethod]
        public void ArithmeticMultiValueConverter_Sum_Test()
        {
            var target = ArithmeticMultiValueConverter.Sum;
            var source = new object[] { 10, 12, "3.5" };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(25.5, result);
        }

        [TestMethod]
        public void ArithmeticMultiValueConverter_Sum_InvalidInput_Test()
        {
            var target = ArithmeticMultiValueConverter.Sum;
            var source = new object[] { 10, 12, "3.5", "NAN" };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(DependencyProperty.UnsetValue, result);
        }

        [TestMethod]
        public void ArithmeticMultiValueConverter_Average_Test()
        {
            var target = ArithmeticMultiValueConverter.Average;
            var source = new object[] { 10, 12, "3.5" };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(8.5, result);
        }

        [TestMethod]
        public void ArithmeticMultiValueConverter_Average_InvalidInput_Test()
        {
            var target = ArithmeticMultiValueConverter.Average;
            var source = new object[] { 10, 12, "3.5", "NAN" };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(DependencyProperty.UnsetValue, result);
        }
    }
}
