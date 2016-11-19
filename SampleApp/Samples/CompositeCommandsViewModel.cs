namespace SampleApp.Samples
{
    using System.Windows;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;

    [VisualCompositionExport(RegionId.Main, Sequence = 4)]
    public class CompositeCommandsViewModel
    {
        [NotNull]
        public ICommand OpenCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Open..."));
            }
        }

        [NotNull]
        public ICommand CloseCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Close..."));
            }
        }

        [NotNull]
        public ICommand CopyCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Copy..."));
            }
        }

        [NotNull]
        public ICommand PasteCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Paste..."));
            }
        }

        [NotNull]
        public ICommand CutCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Cut..."));
            }
        }

        [NotNull]
        public ICommand DeleteCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Delete..."));
            }
        }

        public override string ToString()
        {
            return "Commands";
        }
    }
}
