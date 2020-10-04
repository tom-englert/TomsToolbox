namespace SampleApp.Samples
{
    using System.Windows;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf.Composition.AttributedModel;

    /// <summary>
    /// Interaction logic for Styx.xaml
    /// </summary>
    [DataTemplate(typeof(StyxViewModel))]
    public partial class Styx
    {
        public Styx()
        {
            InitializeComponent();
        }

        private void LargeFont_Checked([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            FontSize = 24;
        }

        private void LargeFont_Unchecked([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            SetValue(FontSizeProperty, DependencyProperty.UnsetValue);
        }
    }
}
