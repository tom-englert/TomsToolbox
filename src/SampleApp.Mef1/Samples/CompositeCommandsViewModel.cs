namespace SampleApp.Mef1.Samples;

using System.Windows;
using System.Windows.Input;

using PropertyChanged;

using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.Mef;

[VisualCompositionExport(RegionId.Main, Sequence = 4)]
[AddINotifyPropertyChangedInterface]
public class CompositeCommandsViewModel
{
    public ICommand OpenCommand => new DelegateCommand(() => MessageBox.Show("Open..."));

    public ICommand CloseCommand => new DelegateCommand(() => MessageBox.Show("Close..."));

    public ICommand CopyCommand => new DelegateCommand(() => MessageBox.Show("Copy..."));

    public ICommand PasteCommand => new DelegateCommand(() => MessageBox.Show("Paste..."));

    public ICommand CutCommand => new DelegateCommand(() => MessageBox.Show("Cut..."));

    public ICommand DeleteCommand => new DelegateCommand(() => MessageBox.Show("Delete..."));

    public override string ToString()
    {
        return "Commands";
    }
}