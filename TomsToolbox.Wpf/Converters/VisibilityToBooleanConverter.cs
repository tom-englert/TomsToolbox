namespace TomsToolbox.Wpf.Converters
{
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// The counterpart of BooleanToVisibilityConverter.
    /// </summary>
    [ValueConversion(typeof(Visibility?), typeof(bool?))]
    public class VisibilityToBooleanConverter : BooleanToVisibilityConverter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public VisibilityToBooleanConverter()
        {
            IsInverted = true;
        }
    }
}
