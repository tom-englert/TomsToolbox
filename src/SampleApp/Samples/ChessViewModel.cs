namespace SampleApp.Samples;

using System.Composition;

using PropertyChanged;

using TomsToolbox.Wpf.Composition.AttributedModel;

[Export]
[VisualCompositionExport(RegionId.Main, Sequence=2)]
[AddINotifyPropertyChangedInterface]
[Shared]
public class ChessViewModel
{
    public override string ToString()
    {
        return "Chess";
    }
}