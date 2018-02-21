namespace SampleApp.Samples
{
    using System.ComponentModel.Composition;

    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for TextBoxView.xaml
    /// </summary>
    [DataTemplate(typeof(TextBoxViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TextBoxView
    {
        public TextBoxView()
        {
            InitializeComponent();
        }
    }
}
