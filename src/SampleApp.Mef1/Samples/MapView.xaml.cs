namespace SampleApp.Mef1.Samples;

using System.ComponentModel.Composition;

using TomsToolbox.Composition;
using TomsToolbox.Wpf.Composition;
using TomsToolbox.Wpf.Composition.Mef;

/// <summary>
/// Interaction logic for MapView.xaml
/// </summary>
[DataTemplate(typeof(MapViewModel))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public partial class MapView
{
    [ImportingConstructor]
    public MapView(IExportProvider exportProvider)
    {
        this.SetExportProvider(exportProvider);

        InitializeComponent();
    }
}