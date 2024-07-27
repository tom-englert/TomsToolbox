namespace SampleApp.Samples;

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.AttributedModel;

[VisualCompositionExport(RegionId.CommandViewContainer)]
internal partial class CompositeCommandChild3ViewModel : INotifyPropertyChanged
{
    public string? Text { get; set; } = "Greetings from child #3";

    public ICommand CopyCommand => new DelegateCommand(() => MessageBox.Show("Copy: " + Text));
}
