namespace SampleApp.Samples
{
    using System.ComponentModel.Composition;

    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for ChessView.xaml
    /// </summary>
    [DataTemplate(typeof(ChessViewModel), Role=TemplateRoles.Content)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ChessView
    {
        public ChessView()
        {
            InitializeComponent();
        }
    }
}
