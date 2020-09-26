namespace SampleApp.Samples
{
    using System.Windows;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using PropertyChanged;

    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.CommandViewContainer)]
    [AddINotifyPropertyChangedInterface]
    class CompositeCommandChild3ViewModel
    {
        [CanBeNull]
        public string Text { get; set; } = "Greetings from child #3";

        [NotNull]
        public ICommand CopyCommand => new DelegateCommand(() => MessageBox.Show("Copy: " + Text));
    }
}
