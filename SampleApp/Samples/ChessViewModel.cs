namespace SampleApp.Samples
{
    using TomsToolbox.Wpf.Composition;

    [VisualCompositionExport(RegionId.Main, Sequence=2)]
    class ChessViewModel
    {
        public override string ToString()
        {
            return "Chess";
        }
    }
}
