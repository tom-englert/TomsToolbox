namespace SampleApp.Samples
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for MiscView.xaml
    /// </summary>
    [DataTemplate(typeof(MiscViewModel), Role = TemplateRoles.Content)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MiscView
    {
        [ImportingConstructor]
        public MiscView(ExportProvider exportProvider)
        {
            this.SetExportProvider(exportProvider);

            InitializeComponent();
        }
    }
}
