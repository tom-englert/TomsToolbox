namespace SampleApp.Samples;

using System.ComponentModel;

using TomsToolbox.Wpf.Composition.AttributedModel;

[VisualCompositionExport(RegionId.Main, Sequence = 4)]
internal partial class SharedWidthSampleViewModel : INotifyPropertyChanged
{
    public string? LongText { get; set; } = "This is a long text";

    public override string ToString()
    {
        return "Alignment";
    }
}
