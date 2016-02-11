﻿namespace TomsToolbox.Wpf.Tests.Converters
{
    using System.Windows;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Wpf.Converters;

    [TestClass]
    public class LogicalMultiValueConverterTests
    {
        [TestMethod]
        public void LogicalMultiValueConverter_And_1_Test()
        {
            var target = LogicalMultiValueConverter.And;
            var source = new object[] { true, true, false };

            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void LogicalMultiValueConverter_And_2_Test()
        {
            var target = LogicalMultiValueConverter.And;
            var source = new object[] { true, true, true };

            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void LogicalMultiValueConverter_And_MixedInput1_Test()
        {
            var target = LogicalMultiValueConverter.And;
            var source = new object[] { true, 1, "true", 2 };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void LogicalMultiValueConverter_And_MixedInput2_Test()
        {
            var target = LogicalMultiValueConverter.And;
            var source = new object[] { true, 1, "true", 0 };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void LogicalMultiValueConverter_Or_1_Test()
        {
            var target = LogicalMultiValueConverter.Or;
            var source = new object[] { true, true, false };

            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void LogicalMultiValueConverter_Or_2_Test()
        {
            var target = LogicalMultiValueConverter.Or;
            var source = new object[] { true, true, true };

            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void LogicalMultiValueConverter_Or_MixedInput1_Test()
        {
            var target = LogicalMultiValueConverter.Or;
            var source = new object[] { true, 1, "true", 0, "false" };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void LogicalMultiValueConverter_Or_MixedInput2_Test()
        {
            var target = LogicalMultiValueConverter.Or;
            var source = new object[] { true, 1, "true", 0 };
            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void LogicalMultiValueConverter_And_InvalidInput_Test()
        {
            var target = LogicalMultiValueConverter.And;
            var source = new object[] { true, 1, "Test" };

            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(DependencyProperty.UnsetValue, result);
        }

        [TestMethod]
        public void LogicalMultiValueConverter_Or_InvalidInput_Test()
        {
            var target = LogicalMultiValueConverter.Or;
            var source = new object[] { true, 1, "Test" };

            var result = target.Convert(source, null, null, null);

            Assert.AreEqual(true, result);
        }
    }
}
