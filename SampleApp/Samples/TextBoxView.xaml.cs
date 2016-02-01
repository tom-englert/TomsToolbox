namespace SampleApp.Samples
{
    using System.ComponentModel.Composition;

    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for TextBoxView.xaml
    /// </summary>
    [DataTemplate(typeof(TextBoxViewModel), Role = TemplateRoles.Content)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TextBoxView : IComposablePart
    {
        public TextBoxView()
        {
            InitializeComponent();
        }
    }
}
