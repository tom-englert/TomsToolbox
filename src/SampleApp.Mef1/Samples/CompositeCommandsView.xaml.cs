namespace SampleApp.Mef1.Samples
{
    using System.ComponentModel.Composition;

    using TomsToolbox.Composition;
    using TomsToolbox.Wpf.Composition;
    using TomsToolbox.Wpf.Composition.Mef;

    /// <summary>
    /// Interaction logic for CompositeCommandsView.xaml
    /// </summary>
    [DataTemplate(typeof(CompositeCommandsViewModel))]
    public partial class CompositeCommandsView
    {
        [ImportingConstructor]
        public CompositeCommandsView(IExportProvider? exportProvider)
        {
            this.SetExportProvider(exportProvider);

            InitializeComponent();
        }
    }
}
