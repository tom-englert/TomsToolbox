namespace SampleApp.Samples;

using TomsToolbox.Wpf.Composition.AttributedModel;

[DataTemplate(typeof(ColorThemeViewModel))]
public partial class ColorThemeView
{
    public ColorThemeView()
    {
        InitializeComponent();
    }
}