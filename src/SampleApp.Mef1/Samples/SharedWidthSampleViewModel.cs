namespace SampleApp.Mef1.Samples;

using System.ComponentModel;

using TomsToolbox.Wpf.Composition.Mef;

[VisualCompositionExport(RegionId.Main, Sequence = 4)]
internal partial class SharedWidthSampleViewModel : INotifyPropertyChanged
{
    public string? LongText { get; set; } = "This is a long text";

    public override string ToString()
    {
        return "Alignment";
    }
}
