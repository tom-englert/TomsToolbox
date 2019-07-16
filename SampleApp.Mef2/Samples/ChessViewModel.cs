namespace SampleApp.Mef2.Samples
{
    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.Main, Sequence=2)]
    class ChessViewModel
    {
        public override string ToString()
        {
            return "Chess";
        }
    }
}
