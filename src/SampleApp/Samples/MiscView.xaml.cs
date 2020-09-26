namespace SampleApp.Samples
{
    using TomsToolbox.Wpf.Composition.AttributedModel;

    /// <summary>
    /// Interaction logic for MiscView.xaml
    /// </summary>
    [DataTemplate(typeof(MiscViewModel))]
    public partial class MiscView
    {
        public MiscView()
        {
            InitializeComponent();
        }
    }
}
