namespace SampleApp.Mef1.Samples;

using PropertyChanged;

using TomsToolbox.Wpf.Composition.Mef;

[VisualCompositionExport(RegionId.Main, Sequence=2)]
[AddINotifyPropertyChangedInterface]
class ChessViewModel
{
    public override string ToString()
    {
        return "Chess";
    }
}