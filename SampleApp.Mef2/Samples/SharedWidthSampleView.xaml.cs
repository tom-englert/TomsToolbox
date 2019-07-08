namespace SampleApp.Mef2.Samples
{
    using TomsToolbox.Wpf.Composition.Mef2;

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
