namespace SampleApp.Samples
{
    using TomsToolbox.Wpf.Composition;

    [VisualCompositionExport(RegionId.Main, Sequence = 3)]
    class TextBoxViewModel : IComposablePart
    {
        public override string ToString()
        {
            return "TextBox";
        }
    }
}
