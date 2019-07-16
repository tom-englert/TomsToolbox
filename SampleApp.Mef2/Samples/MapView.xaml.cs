namespace SampleApp.Mef2.Samples
{
    using System.Composition;

    using JetBrains.Annotations;

    using TomsToolbox.Composition;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;
    using TomsToolbox.Wpf.Composition.Mef2;

    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    [DataTemplate(typeof(MapViewModel))]
    public partial class MapView
    {
        [ImportingConstructor]
        public MapView([NotNull] IExportProvider exportProvider)
        {
            this.SetExportProvider(exportProvider);

            InitializeComponent();
        }
    }
}
