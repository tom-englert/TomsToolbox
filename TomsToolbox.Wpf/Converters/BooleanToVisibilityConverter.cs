namespace TomsToolbox.Wpf.Converters
{
    using System.Windows.Data;

    /// <summary>
    /// A static class hosting a singleton instance of the <see cref="System.Windows.Controls.BooleanToVisibilityConverter"/>
    /// </summary>
    public static class BooleanToVisibilityConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        public static readonly IValueConverter Default = new System.Windows.Controls.BooleanToVisibilityConverter();
    }
}
