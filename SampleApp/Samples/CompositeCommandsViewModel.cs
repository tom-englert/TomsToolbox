namespace SampleApp.Samples
{
    using System.Windows;
    using System.Windows.Input;

    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;

    [VisualCompositionExport(RegionId.Main, Sequence = 4)]
    public class CompositeCommandsViewModel : IComposablePart
    {
        public ICommand OpenCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Open..."));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Close..."));
            }
        }

        public ICommand CopyCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Copy..."));
            }
        }

        public ICommand PasteCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Paste..."));
            }
        }

        public ICommand CutCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Cut..."));
            }
        }

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
