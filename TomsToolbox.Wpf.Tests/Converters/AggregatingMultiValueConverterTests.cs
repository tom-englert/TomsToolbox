namespace TomsToolbox.Wpf.Tests.Converters
{
    using System;
    using System.Windows.Data;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Wpf.Converters;

    [TestClass]
    public class AggregatingMultiValueConverterTests
    {
        [TestMethod]
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

            Assert.AreEqual(true, result);
        }

        [TestMethod]
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

            Assert.AreEqual(false, result);
        }
    }
}
