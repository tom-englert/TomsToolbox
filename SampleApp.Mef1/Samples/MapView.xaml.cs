namespace SampleApp.Mef1.Samples
{
    using System.ComponentModel.Composition;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition.Mef;

    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    [DataTemplate(typeof(MapViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
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
