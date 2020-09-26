namespace SampleApp.Samples
{
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
    }
}
