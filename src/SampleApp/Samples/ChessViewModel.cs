namespace SampleApp.Samples;

using System.ComponentModel;
using System.Composition;

using TomsToolbox.Wpf.Composition.AttributedModel;

[Export]
[VisualCompositionExport(RegionId.Main, Sequence=2)]
[Shared]
public partial class ChessViewModel : INotifyPropertyChanged
{
    public override string ToString()
    {
        return "Chess";
    }
}
