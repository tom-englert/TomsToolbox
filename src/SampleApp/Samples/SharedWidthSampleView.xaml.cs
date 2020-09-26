namespace SampleApp.Samples
{
    using TomsToolbox.Wpf.Composition.AttributedModel;

    /// <summary>
    /// Interaction logic for SharedWidthSampleView.xaml
    /// </summary>
    [DataTemplate(typeof(SharedWidthSampleViewModel))]
    public partial class SharedWidthSampleView
    {
        public SharedWidthSampleView()
        {
            InitializeComponent();
        }
    }
}
