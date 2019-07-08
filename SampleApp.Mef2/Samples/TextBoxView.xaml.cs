namespace SampleApp.Mef2.Samples
{
    using TomsToolbox.Wpf.Composition.Mef2;

    /// <summary>
    /// Interaction logic for TextBoxView.xaml
    /// </summary>
    [DataTemplate(typeof(TextBoxViewModel))]
    public partial class TextBoxView
    {
        public TextBoxView()
        {
            InitializeComponent();
        }
    }
}
