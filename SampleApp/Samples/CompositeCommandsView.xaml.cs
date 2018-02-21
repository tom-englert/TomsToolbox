namespace SampleApp.Samples
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for CompositeCommandsView.xaml
    /// </summary>
    [DataTemplate(typeof(CompositeCommandsViewModel))]
    public partial class CompositeCommandsView
    {
        [ImportingConstructor]
        public CompositeCommandsView([CanBeNull] ExportProvider exportProvider)
        {
            this.SetExportProvider(exportProvider);

            InitializeComponent();
        }
    }
}
