namespace SampleApp.Mef1.Samples
{
    using TomsToolbox.Wpf.Composition.Mef;

    [VisualCompositionExport(RegionId.Main, Sequence = 3)]
    class TextBoxViewModel
    {
        public override string ToString()
        {
            return "TextBox";
        }
    }
}
