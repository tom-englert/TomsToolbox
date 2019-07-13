namespace SampleApp.Mef1.Samples
{
    using TomsToolbox.Wpf.Composition.Mef;

    [VisualCompositionExport(RegionId.Main, Sequence=2)]
    class ChessViewModel
    {
        public override string ToString()
        {
            return "Chess";
        }
    }
}
