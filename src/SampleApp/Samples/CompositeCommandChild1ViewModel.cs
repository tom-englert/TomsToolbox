namespace SampleApp.Samples;

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.AttributedModel;

[VisualCompositionExport(RegionId.CommandViewContainer)]
internal partial class CompositeCommandChild1ViewModel : INotifyPropertyChanged
{
    public string? Text { get; set; } = "Greetings from child #1";

    public ICommand CopyCommand => new DelegateCommand(() => MessageBox.Show("Copy: " + Text));

    public ICommand PasteCommand => new DelegateCommand(() => MessageBox.Show("Paste: " + Text));

    public ICommand CutCommand => new DelegateCommand(() => MessageBox.Show("Cut: " + Text));
}
