namespace SampleApp.Mef2.Samples
{
    using TomsToolbox.Wpf.Composition.Mef2;

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
