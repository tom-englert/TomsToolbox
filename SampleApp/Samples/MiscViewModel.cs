namespace SampleApp.Samples
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using PropertyChanged;

    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition.Mef;

    [VisualCompositionExport(RegionId.Main, Sequence = 99)]
    [ImplementPropertyChanged]
    internal class MiscViewModel
    {
        public override string ToString()
        {
            return "Misc.";
        }

        public DateTime OperationStarted { get; set; } = DateTime.Now;

        public TimeSpan MinimumDuration { get; set; } = TimeSpan.FromMinutes(0.2);

        [NotNull]
        public ICommand ItemsControlDefaultCommand => new DelegateCommand<string>(item => MessageBox.Show(item + " clicked."));

        [NotNull]
        public ICommand GCCollectCommand => new DelegateCommand(GC.Collect);
    }
}