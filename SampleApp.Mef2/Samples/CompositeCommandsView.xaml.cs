namespace SampleApp.Mef2.Samples
{
    using System.Composition;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition.Mef2;

    /// <summary>
    /// Interaction logic for CompositeCommandsView.xaml
    /// </summary>
    [DataTemplate(typeof(CompositeCommandsViewModel))]
    public partial class CompositeCommandsView
    {
        [ImportingConstructor]
        public CompositeCommandsView([CanBeNull] IExportProvider exportProvider)
        {
            this.SetExportProvider(exportProvider);

            InitializeComponent();
        }
    }
}
