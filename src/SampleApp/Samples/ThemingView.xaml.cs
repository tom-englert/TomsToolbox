namespace SampleApp.Samples
{
    using TomsToolbox.Wpf.Composition.AttributedModel;

    [DataTemplate(typeof(ThemingViewModel))]
    public partial class ThemingView
    {
        public ThemingView()
        {
            InitializeComponent();
        }
    }
}
