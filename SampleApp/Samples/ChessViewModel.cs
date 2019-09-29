namespace SampleApp.Samples
{
    using PropertyChanged;

    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.Main, Sequence=2)]
    [ImplementPropertyChanged]
    class ChessViewModel
    {
        public override string ToString()
        {
            return "Chess";
        }
    }
}
