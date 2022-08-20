namespace SampleApp.Samples;

using System.Windows;
using System.Windows.Input;

using PropertyChanged;

using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.AttributedModel;

[VisualCompositionExport(RegionId.CommandViewContainer)]
[AddINotifyPropertyChangedInterface]
class CompositeCommandChild2ViewModel
{
    public string? Text { get; set; } = "Greetings from child #2";

    public ICommand CopyCommand => new DelegateCommand(() => MessageBox.Show("Copy: " + Text));

    public ICommand PasteCommand => new DelegateCommand(() => MessageBox.Show("Paste: " + Text));

    public ICommand CutCommand => new DelegateCommand(() => MessageBox.Show("Cut: " + Text));
}