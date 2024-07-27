namespace SampleApp.Mef1.Samples;

using System.ComponentModel;

using TomsToolbox.Wpf.Composition.Mef;

[VisualCompositionExport(RegionId.Main, Sequence = 3)]
internal partial class TextBoxViewModel : INotifyPropertyChanged
{
    public override string ToString()
    {
        return "TextBox";
    }
}
