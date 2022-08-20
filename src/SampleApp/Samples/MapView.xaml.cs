namespace SampleApp.Samples;

using System.Composition;

using TomsToolbox.Composition;
using TomsToolbox.Wpf.Composition;
using TomsToolbox.Wpf.Composition.AttributedModel;

/// <summary>
/// Interaction logic for MapView.xaml
/// </summary>
[DataTemplate(typeof(MapViewModel))]
public partial class MapView
{
    [ImportingConstructor]
    public MapView(IExportProvider exportProvider)
    {
        this.SetExportProvider(exportProvider);

        InitializeComponent();
    }
}