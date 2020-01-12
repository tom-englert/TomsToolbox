namespace TomsToolbox.Wpf.Tests.Converters
{
    using System.Windows;
    using System.Windows.Data;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Wpf.Converters;

    [TestClass]
    public class BooleanToVisibilityConverterTests
    {
        [NotNull] private static readonly IValueConverter _target = BooleanToVisibilityConverter.Default;
        [NotNull] private static readonly IValueConverter _reference = new System.Windows.Controls.BooleanToVisibilityConverter();

        [TestMethod]
        public void BooleanToVisibilityConverter_EnsureHasSameBehaviorAsSystemConverter_Test()
        {
            VerifyConvert(true);
            VerifyConvert("true");
            VerifyConvert(false);
            VerifyConvert(Visibility.Visible);
            VerifyConvert(Visibility.Hidden);
            VerifyConvert(Visibility.Collapsed);
            VerifyConvert(null);
            VerifyConvert(DependencyProperty.UnsetValue);
        }

        private static void VerifyConvert([CanBeNull] object? source)
        {
            var expected = _reference.Convert(source, null, null, null);
            var result = _target.Convert(source, null, null, null);

            Assert.AreEqual(expected, result);

            expected = _reference.ConvertBack(source, null, null, null);
            result = _target.ConvertBack(source, null, null, null);

            Assert.AreEqual(expected, result);
        }
    }
}
