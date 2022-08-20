namespace SampleApp.Mef1.Samples;

using PropertyChanged;

using TomsToolbox.Wpf.Composition.Mef;

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