namespace SampleApp.Samples
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using PropertyChanged;

    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.Main, Sequence = 99)]
    [AddINotifyPropertyChangedInterface]
    internal class MiscViewModel
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
}