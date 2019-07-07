namespace SampleApp.Samples
{
    using TomsToolbox.Wpf.Composition.Mef;

    /// <summary>
    /// Interaction logic for CommandViewChild2.xaml
    /// </summary>
    [DataTemplate(typeof(CompositeCommandChild2ViewModel))]
    public partial class CompositeCommandChild2View
    {
        public CompositeCommandChild2View()
        {
            InitializeComponent();
        }
    }
}
