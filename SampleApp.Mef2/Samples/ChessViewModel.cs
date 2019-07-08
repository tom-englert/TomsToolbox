namespace SampleApp.Mef2.Samples
{
    using TomsToolbox.Wpf.Composition.Mef2;

    [VisualCompositionExport(RegionId.Main, Sequence=2)]
    class ChessViewModel
    {
        public override string ToString()
        {
            return "Chess";
        }
    }
}
