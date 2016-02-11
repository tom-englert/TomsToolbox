namespace TomsToolbox.Wpf.Tests.Converters
{
    using System.Windows;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Wpf.Converters;

    [TestClass]
    public class BooleanToVisibilityConverterTests
    {
        private static readonly BooleanToVisibilityConverter _target = new BooleanToVisibilityConverter();
        private static readonly System.Windows.Controls.BooleanToVisibilityConverter _reference = new System.Windows.Controls.BooleanToVisibilityConverter();

        [TestMethod]
        public void BooleanToVisibilityConverter_EnsureSameBehaviorAsSysteConverterTest()
        {
            Verify(true);
            Verify("true");
            Verify(false);
            Verify(null);
            Verify(DependencyProperty.UnsetValue);
        }

        private static void Verify(object source)
        {
            var expected = _reference.Convert(source, null, null, null);
            var result = _target.Convert(source, null, null, null);

            Assert.AreEqual(result, expected);
        }
    }
}
