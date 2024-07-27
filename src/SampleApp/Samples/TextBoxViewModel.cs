namespace SampleApp.Samples;

using System.ComponentModel;

using TomsToolbox.Wpf.Composition.AttributedModel;

[VisualCompositionExport(RegionId.Main, Sequence = 3)]
internal partial class TextBoxViewModel : INotifyPropertyChanged
{
    public override string ToString()
    {
        return "TextBox";
    }
}
