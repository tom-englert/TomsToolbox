namespace SampleApp.Samples
{
    using System.Windows;
    using System.Windows.Input;

    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;

    [VisualCompositionExport(RegionId.CommandViewContainer)]
    class CompositeCommandChild2ViewModel : ObservableObject
    {
        private string _text = "Greetings from child #2";

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetProperty(ref _text, value, nameof(Text));
            }
        }


        public ICommand CopyCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Copy: " + Text));
            }
        }

        public ICommand PasteCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Paste: " + Text));
            }
        }

        public ICommand CutCommand
        {
            get
            {
                return new DelegateCommand(() => MessageBox.Show("Cut: " + Text));
            }
        }
    }
}
