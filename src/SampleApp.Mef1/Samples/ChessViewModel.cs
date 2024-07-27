namespace SampleApp.Mef1.Samples;

using System.ComponentModel;

using TomsToolbox.Wpf.Composition.Mef;

[VisualCompositionExport(RegionId.Main, Sequence=2)]
internal partial class ChessViewModel : INotifyPropertyChanged
{
    public override string ToString()
    {
        return "Chess";
    }
}
