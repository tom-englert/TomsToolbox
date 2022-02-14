namespace SampleApp.Samples
{
    using TomsToolbox.Wpf.Composition.AttributedModel;

    /// <summary>
    /// Interaction logic for CharView.xaml
    /// </summary>
    [DataTemplate(typeof(ChartViewModel))]
    public partial class CharView
    {
        public CharView()
        {
            InitializeComponent();
        }
    }
}
