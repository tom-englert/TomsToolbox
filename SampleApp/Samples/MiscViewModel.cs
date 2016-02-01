namespace SampleApp.Samples
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    using TomsToolbox.Wpf.Composition;

    [VisualCompositionExport(RegionId.Main, Sequence = 99)]
    internal class MiscViewModel : IComposablePart
    {
        public override string ToString()
        {
            return "Misc.";
        }
    }
}