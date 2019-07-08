namespace SampleApp.Mef2.Samples
{
    using TomsToolbox.Wpf.Composition.Mef2;

    /// <summary>
    /// Interaction logic for CommandViewChild1.xaml
    /// </summary>
    [DataTemplate(typeof(CompositeCommandChild1ViewModel))]
    public partial class CompositeCommandChild1View
    {
        public CompositeCommandChild1View()
        {
            InitializeComponent();
        }
    }
}
