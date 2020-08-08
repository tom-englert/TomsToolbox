namespace SampleApp.Samples
{
    using PropertyChanged;

    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.Main, Sequence=9)]
    [AddINotifyPropertyChangedInterface]
    class StyxViewModel
    {
        public override string ToString()
        {
            return "Styx";
        }
    }
}
