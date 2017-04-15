namespace SampleApp.Samples
{
    using JetBrains.Annotations;

    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf.Composition;

    [VisualCompositionExport(RegionId.Main, Sequence = 4)]
    class SharedWidthSampleViewModel : ObservableObject
    {
        private string _longText = "This is a long text";

        [CanBeNull]
        public string LongText
        {
            get
            {
                return _longText;
            }
            set
            {
                SetProperty(ref _longText, value, () => LongText);
            }
        }

        public override string ToString()
        {
            return "Alignment";
        }
    }
}
