namespace SampleApp.Mef1.Samples;

using PropertyChanged;

using TomsToolbox.Wpf.Composition.Mef;

[VisualCompositionExport(RegionId.Main, Sequence = 3)]
[AddINotifyPropertyChangedInterface]
class TextBoxViewModel
{
    public override string ToString()
    {
        return "TextBox";
    }
}