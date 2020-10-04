namespace SampleApp.Samples
{
    using System.Composition;

    using SampleApp;

    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.Main, Sequence = 100)]
    [Export, Shared]
    public class ColorViewModel
    {
        public override string ToString()
        {
            return "Colors";
        }
    }
}
