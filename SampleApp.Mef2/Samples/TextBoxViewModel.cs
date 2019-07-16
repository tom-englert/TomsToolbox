namespace SampleApp.Mef2.Samples
{
    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.Main, Sequence = 3)]
    class TextBoxViewModel
    {
        public override string ToString()
        {
            return "TextBox";
        }
    }
}
