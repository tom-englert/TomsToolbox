namespace SampleApp.Samples
{
    using PropertyChanged;

    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.Main, Sequence = 3)]
    [ImplementPropertyChanged]
    class TextBoxViewModel
    {
        public override string ToString()
        {
            return "TextBox";
        }
    }
}
