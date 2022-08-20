namespace SampleApp.Samples;

using PropertyChanged;

using TomsToolbox.Wpf.Composition.AttributedModel;

[VisualCompositionExport(RegionId.Main, Sequence = 4)]
[AddINotifyPropertyChangedInterface]
class SharedWidthSampleViewModel
{
    public string? LongText { get; set; } = "This is a long text";

    public override string ToString()
    {
        return "Alignment";
    }
}