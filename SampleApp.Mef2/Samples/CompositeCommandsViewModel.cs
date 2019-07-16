namespace SampleApp.Mef2.Samples
{
    using System.Windows;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.Main, Sequence = 4)]
    public class CompositeCommandsViewModel
    {
        [NotNull]
        public ICommand OpenCommand => new DelegateCommand(() => MessageBox.Show("Open..."));

        [NotNull]
        public ICommand CloseCommand => new DelegateCommand(() => MessageBox.Show("Close..."));

        [NotNull]
        public ICommand CopyCommand => new DelegateCommand(() => MessageBox.Show("Copy..."));

        [NotNull]
        public ICommand PasteCommand => new DelegateCommand(() => MessageBox.Show("Paste..."));

        [NotNull]
        public ICommand CutCommand => new DelegateCommand(() => MessageBox.Show("Cut..."));

        [NotNull]
        public ICommand DeleteCommand => new DelegateCommand(() => MessageBox.Show("Delete..."));

        public override string ToString()
        {
            return "Commands";
        }
    }
}
