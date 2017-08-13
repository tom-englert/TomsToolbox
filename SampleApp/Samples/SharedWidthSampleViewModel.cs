namespace SampleApp.Samples
{
    using JetBrains.Annotations;

    using PropertyChanged;

    using TomsToolbox.Wpf.Composition;

    [VisualCompositionExport(RegionId.Main, Sequence = 4)]
    [AddINotifyPropertyChangedInterface]
    class SharedWidthSampleViewModel
    {
        [CanBeNull]
        public string LongText { get; set; } = "This is a long text";

        public override string ToString()
        {
            return "Alignment";
        }
    }
}
