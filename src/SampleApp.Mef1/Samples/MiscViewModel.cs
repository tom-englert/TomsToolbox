namespace SampleApp.Mef1.Samples;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.Mef;

[VisualCompositionExport(RegionId.Main, Sequence = 99)]
internal partial class MiscViewModel : INotifyPropertyChanged
{
    public override string ToString()
    {
        return "Misc.";
    }

    public DateTime OperationStarted { get; set; } = DateTime.Now;

    public TimeSpan MinimumDuration { get; set; } = TimeSpan.FromMinutes(0.2);

    public ICommand ItemsControlDefaultCommand => new DelegateCommand<string>(item => MessageBox.Show(item + " clicked."));

    public ICommand GCCollectCommand => new DelegateCommand(GC.Collect);
}
