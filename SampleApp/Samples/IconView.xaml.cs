namespace SampleApp.Samples
{
    using TomsToolbox.Wpf.Composition.AttributedModel;

    /// <summary>
    /// Interaction logic for IconView.xaml
    /// </summary>
    [DataTemplate(typeof(IconViewModel))]
    public partial class IconView
    {
        public IconView()
        {
            InitializeComponent();
        }
    }

    [VisualCompositionExport(RegionId.Main, Sequence = 5)]
    public class IconViewModel
    {
        public override string ToString()
        {
            return "Icon";
        }
    }
}
