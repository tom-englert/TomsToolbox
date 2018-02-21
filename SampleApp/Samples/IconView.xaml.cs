namespace SampleApp.Samples
{
    using System.Windows.Controls;

    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for IconView.xaml
    /// </summary>
    [DataTemplate(typeof(IconViewModel))]
    public partial class IconView : UserControl
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
